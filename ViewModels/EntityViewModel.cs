using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using WpfGame.Models;
using WpfGame.Models.Entities;

namespace WpfGame.ViewModels;

public class EntityViewModel : ReactiveObject
{
    public Frames Frames { get; }
    public Entity Entity { get; }
    public bool IsLastMoveToLeft { get; private set; }
    private BitmapSource _source;
    private BitmapSource? FirstDeathView { get; set; }
    private bool IsCurrentViewChangedAfterFirstDeathViewInit { get; set; } // :(
    public bool IsDeath { get; private set; }
    public int CounterShootFrames { get; set; }
    public BitmapSource? PreviousNextShootFrame { get; set; }
    
    public BitmapSource Source
    {
        get => _source;
        private set => this.RaiseAndSetIfChanged(ref _source, value);
    }
    
    public EntityViewModel(Frames frames, Entity entity)
    {
        Entity = entity;
        IsCurrentViewChangedAfterFirstDeathViewInit = false;
        FirstDeathView = null;
        _source = frames.GetCurrent(entity.EntityState);
        Frames = frames;
    }
    
    public void MoveNextFrame(Size cellSize)
    {
        if (IsDeath) return;
        var nextFrame =  Frames.GetCurrent(Entity.EntityState);
        switch (Entity.EntityState)
        {
            case EntityState.Death when FirstDeathView == null:
                FirstDeathView = nextFrame;
                break;
            case EntityState.Death when FirstDeathView != nextFrame && !IsCurrentViewChangedAfterFirstDeathViewInit:
                IsCurrentViewChangedAfterFirstDeathViewInit = true;
                break;
            case EntityState.Death when FirstDeathView == nextFrame && IsCurrentViewChangedAfterFirstDeathViewInit:
                IsDeath = true;
                return;
            case EntityState.Shoot when Entity is Player player && PreviousNextShootFrame != nextFrame:
                if (CounterShootFrames == 4)
                {
                    CounterShootFrames = 0;
                    PreviousNextShootFrame = null;
                    player.Stand();
                    return;
                }

                PreviousNextShootFrame = nextFrame;
                CounterShootFrames++;
                break;
        }

        if (Entity.Direction.X < 0 || (Entity.Direction.Equals(new Point(0, 0)) && IsLastMoveToLeft))
        {
            nextFrame = new TransformedBitmap(nextFrame, new ScaleTransform(-1, 1, 0, 0));
            IsLastMoveToLeft = true;
        }
        else if (Entity.Direction.X > 0)
            IsLastMoveToLeft = false;

        var view = DrawView(nextFrame, cellSize);
        Source = view;
    }

    private BitmapSource DrawView(BitmapSource nextFrame, Size cellSize)
    {
        void DrawFrameHealthBar(DrawingContext drawingContext, Rect healthBar)
        {
            var healthBarPen = new Pen(Brushes.Bisque, 1.5);
            drawingContext.DrawLine(healthBarPen, healthBar.TopLeft, healthBar.TopRight);
            drawingContext.DrawLine(healthBarPen, healthBar.TopRight, healthBar.BottomRight);
            drawingContext.DrawLine(healthBarPen, healthBar.BottomRight, healthBar.BottomLeft);
            drawingContext.DrawLine(healthBarPen, healthBar.BottomLeft, healthBar.TopLeft);
        }

        var drawingVisual = new DrawingVisual();
        var pixelWidth = nextFrame.PixelWidth + (int)cellSize.Width / 2;
        var pixelHeight = nextFrame.PixelHeight + (int)cellSize.Height / 2;
        var renderTargetBitmap = new RenderTargetBitmap(pixelWidth,pixelHeight, 96, 96, PixelFormats.Default);
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawImage(nextFrame, new Rect(10, 20, nextFrame.Width, nextFrame.Height));
            var healthBar = new Rect(5, 5,
                cellSize.Width * 1.05, cellSize.Height / 4.4);
            drawingContext.DrawRectangle(Brushes.OrangeRed, new Pen(Brushes.Aqua, 0),
                healthBar with{Width = healthBar.Width / 100 * Entity.Health});
            DrawFrameHealthBar(drawingContext, healthBar);
        }
        
        renderTargetBitmap.Render(drawingVisual);
        return renderTargetBitmap;
    }
}
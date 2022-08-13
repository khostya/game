using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using Splat;
using WpfGame.Extensions;
using WpfGame.Models;
using WpfGame.Models.Entities;
using WpfGame.Models.Game;
using WpfGame.Models.Map;

namespace WpfGame.ViewModels;

public class GameViewModel : RoutingViewModelBase
{
    public Game Game { get; }
    private Size CellSize { get; }
    private Settings Settings { get; }
    public EntityViewModel PlayerView { get; }
    public EntityViewModel[] EntityView { get; }
    private List<BulletViewModel> Bullets { get; }
    public ReactiveImageSource MapView { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoMenu { get; }

    public void AddBullet(BulletViewModel bulletViewModel)
    {
        Game.AddBullet(bulletViewModel.Bullet);
        Bullets.Add(bulletViewModel);
    }
    
    public GameViewModel(IScreen? screen = null) : base(screen)
    {
        Settings = Locator.Current.GetService<Settings>()!;
        GoMenu = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new StartViewModel()));
        var frames = Resources.AllFrames;
        CellSize = Locator.Current.GetService<Size>();
        Game = new Game(Resources.MapLevels.StartLevel);
        EntityView = Game.Entities.Select(x => new EntityViewModel(frames[x.EntityType], x)).ToArray();
        PlayerView = new EntityViewModel(frames[EntityType.Player], Game.Player);
        MapView = new ReactiveImageSource(DrawMap());
        Bullets = new List<BulletViewModel>();
        Observable.Interval(Settings.UpdateInterval)
            .ObserveOnDispatcher()
            .Subscribe(_ => MoveEntitiesAndFighting());
        Observable.Interval(Settings.UpdateInterval / 5)
            .ObserveOnDispatcher()
            .Subscribe(_ => MoveBullets());
    }

    private void MoveEntitiesAndFighting()
    {
        Game.MoveEntitiesAndFighting();
        DrawEntities();
    }
    
    private async void MoveBullets() => await Task.Run(() => Game.MoveBullets());

    private void DrawEntities()
    {
        EntityView.Append(PlayerView).ForEach(x => x.MoveNextFrame(CellSize));
        Bullets.ForEach(x => x.MoveNextFrame());
    }

    private RenderTargetBitmap DrawMap()
    {
        void DrawCell(Rect rectForDraw, (int X, int Y, MapCell Cell) cell, DrawingContext drawingContext)
        {
            switch (cell.Cell)
            {
                case MapCell.Wall:
                    drawingContext.DrawImage(Resources.MapSprites.Wall, rectForDraw);
                    break; 
                case MapCell.Player or MapCell.Floor or MapCell.Crab:
                    drawingContext.DrawImage(Resources.MapSprites.Floor, rectForDraw);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        
        var pixelWidth = Game.Map.Width * (int)CellSize.Width;
        var pixelHeight = Game.Map.Height * (int)CellSize.Height;
        var renderTargetBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Default);
        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            foreach (var cell in Game.Map.Cells)
            {
                var location = new Point(cell.X * CellSize.Width, cell.Y * CellSize.Height);
                var rectForDraw = new Rect(location, CellSize);
                DrawCell(rectForDraw, cell, drawingContext);
            }
        }
        
        renderTargetBitmap.Render(drawingVisual);
        return renderTargetBitmap;
    }
}
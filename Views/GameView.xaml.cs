using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ReactiveUI;
using Splat;
using WpfGame.Extensions;
using WpfGame.Models;
using WpfGame.Models.Entities;
using WpfGame.ViewModels;

namespace WpfGame.Views;

public partial class GameView : ReactiveUserControl<GameViewModel>, IComponentConnector
{
    private ReactivePoint CameraOffset { get; }
    private Size CellSize { get; }
    private Point PreviousPlayerPosition { get; set; }

    public GameView()
    {
        InitializeComponent();
        ViewModel = new GameViewModel();
        CameraOffset = new ReactivePoint(0, 0);
        InitCameraOffset();
        PreviousPlayerPosition = new Point(ViewModel.Game.Player.Position.X, ViewModel.Game.Player.Position.Y);
        CellSize = Locator.Current.GetService<Size>();
        RegisterEvents();
        RegisterEntities();
        ViewModel.Game.Player.Position.WhenAnyValue(x => x.X, x => x.Y,
                (x, y) => new Point(x, y)).ObserveOnDispatcher()
            .Subscribe(position =>
            {
                UpdateCameraOffset(position);
                ChangePositionOnCanvas(Map, new Point(0, 0));
                ChangePositionOnCanvas(Player, position);
                PreviousPlayerPosition = position;
            });
        RegisterOptionsCommand();
        this.BindCommand(ViewModel, vm => vm.GoMenu, v => v.Menu);
        this.OneWayBind(ViewModel, vm => (ImageSource)vm.PlayerView.Source, v => v.Player.Source);
        this.OneWayBind(ViewModel, vm => (ImageSource)vm.MapView.Source, v => v.Map.Source);
    }

    private void RegisterOptionsCommand()
    {
        var closingCommand = ReactiveCommand.Create<OptionsInGameView>(x => Game.Children.Remove(x));
        var optionsInGame = new OptionsInGameView(closingCommand)
        {
            Height = App.Current.MainWindow.Height / 2, Width = App.Current.MainWindow.Width / 2
        };
        Options.Command = ReactiveCommand.Create(() =>
        {
            Game.Children.Add(optionsInGame);
            Canvas.SetLeft(optionsInGame, App.Current.MainWindow.Width / 4);
            Canvas.SetTop(optionsInGame, App.Current.MainWindow.Height / 4);
        });
    }
    
    private void RegisterEntities()
    {
        var settings = Locator.Current.GetService<Settings>()!;
        var entityViewModels = ViewModel!.EntityView;
        var images = entityViewModels.Select(_ => new Image()).ToArray();
        images.ForEach(x => Game.Children.Insert(1, x));
        entityViewModels.ForEach((vm, i) =>
        {
            var cameraOffsetChanged = CameraOffset
                .WhenAnyValue(x => x.X, x => x.Y)
                .Select(_ => vm.Entity.Position);
            
            vm.Entity.Position.WhenAnyValue(x => x.X, x => x.Y)
                .Select(_ => vm.Entity.Position)
                .Merge(cameraOffsetChanged)
                .Select(x => new Point(x.X, x.Y + CellSize.Height / 2))
                .ObserveOnDispatcher()
                .Subscribe(x => ChangePositionOnCanvas(images[i], x));
        });
        Observable.Interval(settings.UpdateInterval)
            .ObserveOnDispatcher().Subscribe(_ =>
            {
                images.ForEach((x, i) => x.Source = ViewModel!.EntityView[i].Source);
            });
    }
    
    private void InitCameraOffset()
    {
        var windowHeight = App.Current.MainWindow.Height;
        var windowWidth = App.Current.MainWindow.Width;
        var map = ViewModel!.Game.Map;
        if (PreviousPlayerPosition.X > windowWidth / 2 &&
            PreviousPlayerPosition.X < CellSize.Width * (map.Width + 0.40) - windowWidth / 2)
            CameraOffset.X = PreviousPlayerPosition.X;
        if (PreviousPlayerPosition.Y > windowHeight / 2 &&
            PreviousPlayerPosition.Y < CellSize.Height * (map.Height + 1) - windowHeight / 2)
            CameraOffset.Y = PreviousPlayerPosition.Y;
    }
    
    private void ChangePositionOnCanvas(UIElement uiElement, Point position)
    {
        Canvas.SetLeft(uiElement, CameraOffset.X + position.X);
        Canvas.SetTop(uiElement, CameraOffset.Y + position.Y);
    }
    
    private void UpdateCameraOffset(Point newPlayerPosition)
    {
        var playerPosition = ViewModel!.Game.Player.Position;
        var windowHeight = App.Current.MainWindow.Height;
        var windowWidth = App.Current.MainWindow.Width;
        var map = ViewModel.Game.Map;
        if (playerPosition.X > windowWidth / 2 && playerPosition.X < CellSize.Width * (map.Width + 0.40) - windowWidth / 2)
            CameraOffset.X += PreviousPlayerPosition.X - newPlayerPosition.X;
        if (playerPosition.Y > windowHeight / 2 && playerPosition.Y < CellSize.Height * (map.Height + 1) - windowHeight / 2)
            CameraOffset.Y += PreviousPlayerPosition.Y - newPlayerPosition.Y;
    }
    
    private void RegisterEvents()
    {
        this.WhenActivated(d =>
        {
            var window = Application.Current.MainWindow;
            window.Events().MouseDown.Where(x => x.ChangedButton == MouseButton.Left)
                .Select(x => x.GetPosition(Player))
                .Subscribe(OnPlayerShoot);
            window.Events().KeyDown.Select(x => x.Key).Subscribe(OnKeyDown).DisposeWith(d);
            window.Events().KeyUp.Select(x => x.Key).Subscribe(OnKeyUp).DisposeWith(d);
        });
    }

    private void OnPlayerShoot(Point pointForShoot)
    {
        var playerState = ViewModel!.Game.Player.EntityState;
        if (playerState is not EntityState.Standing and not EntityState.Attack) return;
        if (pointForShoot.Equals(new Point(0, 0))) return;
        var maxCoordinate = Math.Abs(Math.Max(pointForShoot.X, pointForShoot.Y));
        if (maxCoordinate == 0)
        {
            maxCoordinate = 1;
            if (pointForShoot.X < pointForShoot.Y)
                pointForShoot.X = -1;
            else if (pointForShoot.Y < pointForShoot.X)
                pointForShoot.Y = -1;
        }

        ViewModel!.Game.Player.Shoot();
        var direction = new Point(pointForShoot.X / maxCoordinate, pointForShoot.Y / maxCoordinate);
        AddBullet(direction);
    }
    
    private void AddBullet(Point direction)
    {
        var cellSizeInView = Locator.Current.GetService<Size>();
        var playerPosition = ViewModel!.Game.Player.Position;
        var centerY = playerPosition.Y + cellSizeInView.Height / 2;
        var bulletPosition = ViewModel.PlayerView.IsLastMoveToLeft 
            ? new ReactivePoint(playerPosition.X, centerY)
            : new ReactivePoint(playerPosition.X + cellSizeInView.Width, centerY) ;
        var bullet = new Bullet(bulletPosition, direction);
        var bulletViewModel = new BulletViewModel(Models.Resources.AllFrames[EntityType.Bullet], bullet);
        var imageBullet = new Image();
        Game.Children.Add(imageBullet);
        var cameraOffsetChanged = CameraOffset
            .WhenAnyValue(x => x.X, x => x.Y)
            .Select(_ => new Point(bullet.Position.X, bullet.Position.Y));
        
        bullet.Position.WhenAnyValue(x => x.X, x => x.Y,
                (x, y) => new Point(x, y)).Merge(cameraOffsetChanged).ObserveOnDispatcher()
            .Subscribe(x => ChangePositionOnCanvas(imageBullet, x));
        var settings = Locator.Current.GetService<Settings>()!;
        
        Observable.Interval(settings.UpdateInterval)
            .ObserveOnDispatcher().Subscribe(_ => imageBullet.Source = bulletViewModel.Source);
        ViewModel.AddBullet(bulletViewModel);
    }
    
    
    private void OnKeyDown(Key key)
    {
        var player = ViewModel!.Game.Player;
        switch (key)
        {
            case Key.A:
                player.Direction = player.Direction with {X = -1};
                break;
            case Key.D:
                player.Direction = player.Direction with {X = 1};
                break;
            case Key.W:
                player.Direction = player.Direction with {Y = -1};
                break;
            case Key.S:
                player.Direction = player.Direction with {Y = 1};
                break;
            case Key.Space:
                player.Attack();
                break;
        }
    }

    private void OnKeyUp(Key key)
    {
        switch (key)
        {
            case Key.A:
            case Key.D:
                ViewModel!.Game.Player.Direction = ViewModel.Game.Player.Direction with {X = 0};
                break;
            case Key.W:
            case Key.S:
                ViewModel!.Game.Player.Direction = ViewModel.Game.Player.Direction with {Y = 0};
                break;
            case Key.Space:
                ViewModel!.Game.Player.StopAttack();
                break;
        }
    }
}
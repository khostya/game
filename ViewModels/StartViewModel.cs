using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Imaging;
using ReactiveUI;
using Splat;
using WpfGame.Models;
using WpfGame.Views;

namespace WpfGame.ViewModels;

public class StartViewModel : RoutingViewModelBase
{
    public ReactiveCommand<Unit, IRoutableViewModel> GoOptions { get; }

    public ReactiveCommand<Unit, IRoutableViewModel> GoGame { get; }
    public ReactiveCommand<Unit, Unit> CloseWindow { get; }
    public ReactiveCommand<Unit, Unit> MuteMusic { get; }
    public ReactiveImageSource MuteButtonView { get; }
    
    public StartViewModel(IScreen? screen = null) : base(screen)
    {
        var settings = Locator.Current.GetService<Settings>()!;
        MuteButtonView = new ReactiveImageSource(GetCurrentMoveButtonView(settings.IsMusicMute)); 
        MuteMusic = ReactiveCommand.Create<Unit, Unit>(_ =>
        {
            settings.IsMusicMute = !settings.IsMusicMute;
            MuteButtonView.Source = GetCurrentMoveButtonView(settings.IsMusicMute);
            return default;
        });
        GoOptions = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new OptionsViewModel()));
        GoGame = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new GameViewModel()));
        CloseWindow = ReactiveCommand.Create(() => App.Current.MainWindow.Close());
    }

    private static BitmapSource GetCurrentMoveButtonView(bool isMusicMute)
    {
        return isMusicMute ? Resources.Icons.Mute16px : Resources.Icons.SoundOn16px;
    }
}
using System.Reactive;
using System.Windows.Media.Imaging;
using ReactiveUI;
using Splat;
using WpfGame.Models;

namespace WpfGame.ViewModels;

public class OptionsViewModel : RoutingViewModelBase
{
    public ReactiveCommand<Unit, Unit> MuteMusic { get; }
    public ReactiveImageSource MuteButtonView { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoMenu { get; }

    public OptionsViewModel(IScreen? screen = null) : base(screen)
    {
        var settings = Locator.Current.GetService<Settings>()!;
        MuteButtonView = new ReactiveImageSource(GetCurrentMoveButtonView(settings.IsMusicMute)); 
        MuteMusic = ReactiveCommand.Create<Unit, Unit>(_ =>
        {
            settings.IsMusicMute = !settings.IsMusicMute;
            MuteButtonView.Source = GetCurrentMoveButtonView(settings.IsMusicMute);
            return default;
        });
        GoMenu = ReactiveCommand
            .CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new StartViewModel()));
    }

    private BitmapSource GetCurrentMoveButtonView(bool isMusicMute)
    {
        return isMusicMute ? Resources.Icons.Mute16px : Resources.Icons.SoundOn16px;
    }
}
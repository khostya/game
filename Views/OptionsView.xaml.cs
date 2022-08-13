using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Markup;
using ReactiveUI;
using Splat;
using WpfGame.Models;
using WpfGame.ViewModels;

namespace WpfGame.Views;

public partial class OptionsView : ReactiveUserControl<OptionsViewModel>, IComponentConnector
{
    private Settings Settings { get; }
    public ReactiveCommand<Unit, Unit> RemoveMenuCommand { get; }
    
    public OptionsView()
    {
        InitializeComponent();
        Settings = Locator.Current.GetService<Settings>()!;
        RemoveMenuCommand = ReactiveCommand.Create(() => Options.Children.Remove(Button_GoMenu));
        this.BindCommand(ViewModel, vm => vm.GoMenu, v => v.Button_GoMenu);
        this.BindCommand(ViewModel, vm => vm.MuteMusic, v => v.Button_MuteMusic);
        this.OneWayBind(ViewModel, vm => vm.MuteButtonView.Source,
            v => v.Button_MuteMusicImage.Source);
        MusicVolume.Value = Settings.MusicVolume;
    }

    private void MusicVolume_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        Settings.MusicVolume = e.NewValue;
    }
}
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using ReactiveUI;
using WpfGame.ViewModels;

namespace WpfGame.Views;

public partial class StartView : ReactiveUserControl<StartViewModel>, IComponentConnector
{
    public StartView()
    {
        InitializeComponent();
        ViewModel = new StartViewModel();
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.GoGame, v => v.GoGame_Button)
                .DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.GoOptions, v => v.Options_Button)
                .DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.CloseWindow, v => v.Exit_Button)
                .DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.MuteMusic, v => v.MuteMusic_Batton)
                .DisposeWith(d);
            this.OneWayBind(ViewModel, vm => vm.MuteButtonView.Source, 
                    v => v.MuteMusicContent.Source)
                .DisposeWith(d);
        });
    }
}
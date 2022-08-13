using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using WpfGame.ViewModels;
using WpfGame.Views;

namespace WpfGame;

public partial class OptionsInGameView : UserControl
{
    public OptionsInGameView(ReactiveCommand<OptionsInGameView, Unit> closingCommand)
    {
        InitializeComponent();
        Close.Command = ReactiveCommand.Create(() => closingCommand.Execute(this).Subscribe());
        var optionsView = new OptionsView
            {ViewModel = new OptionsViewModel(), Width = Width, Margin = new Thickness(0, 40, 0, 0)};
        DockPanel.SetDock(optionsView, Dock.Top);
        optionsView.RemoveMenuCommand.Execute().Subscribe();
        DockPanel.Children.Insert(0, optionsView);
    }
}
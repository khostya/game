using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using ReactiveUI;
using Splat;
using WpfGame.Models;
using WpfGame.ViewModels;

namespace WpfGame.Views
{
    public partial class MainWindow : ReactiveWindow<MainViewModel>, IComponentConnector
    {
        public ReactiveCommand<bool, Unit> OnMuteMusic { get; }
        public ReactiveCommand<double, Unit> ChangeVolume { get; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            OnMuteMusic = ReactiveCommand.Create<bool>(isMuted =>
            {
                switch (isMuted)
                {
                    case true when Music.CanPause:
                        Music.Pause();
                        break;
                    case false:
                        Music.Play();
                        break;
                }
            });
            Music.Volume = Locator.Current.GetService<Settings>()!.MusicVolume;
            ChangeVolume = ReactiveCommand.Create<double>(newVolume => Music.Volume = newVolume);
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                    .DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.MusicUri, v => v.Music.Source)
                    .DisposeWith(d);
            });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close(); 
        }

        private void Music_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            Music.Position = TimeSpan.Zero;
            Music.Play();
        }
    }
}
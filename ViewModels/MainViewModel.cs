using System;
using System.IO;
using ReactiveUI;
using Splat;

namespace WpfGame.ViewModels;

public class MainViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; }
    public Uri MusicUri { get; }
    public MainViewModel()
    {
        Router = new RoutingState();
        var pathToMusic = Path.Combine(
            new DirectoryInfo(Directory.GetCurrentDirectory()).Parent!.Parent!.Parent!.ToString(), "music.mp3");
        MusicUri = new Uri(pathToMusic, UriKind.Absolute);
        Locator.CurrentMutable.Register<IScreen>(() => this);
    }
}
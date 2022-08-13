using System;
using ReactiveUI;
using WpfGame.Views;

namespace WpfGame.Models;

public class Settings : ReactiveObject
{
    public TimeSpan UpdateInterval { get; } = TimeSpan.FromMilliseconds(20);

    private bool _isMusicMute;

    public bool IsMusicMute
    {
        get => _isMusicMute;
        set
        {
            if (_isMusicMute != value) 
                ((MainWindow) App.Current.MainWindow).OnMuteMusic.Execute(value).Subscribe();
            _isMusicMute = value;
        }
    }

    private double _musicVolume;

    public double MusicVolume
    {
        get => _musicVolume;
        set
        {
            if (!_musicVolume.Equals(value))
                ((MainWindow) App.Current.MainWindow).ChangeVolume.Execute(value).Subscribe();
            _musicVolume = value;
        }
    }

    public Settings(bool isMusicMute = true)
    {
        _musicVolume = 0.5;
        _isMusicMute = isMusicMute;
    }
    
    public Settings(TimeSpan updateInterval, bool isMusicMute = true) : this(isMusicMute)
    {
        UpdateInterval = updateInterval;
    }
}
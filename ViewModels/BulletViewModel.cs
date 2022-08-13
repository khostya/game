using System.Windows.Media.Imaging;
using ReactiveUI;
using WpfGame.Models;
using WpfGame.Models.Entities;

namespace WpfGame.ViewModels;

public class BulletViewModel : ReactiveObject
{
    public Frames Frames { get; }
    public Bullet Bullet { get; }
    private BitmapSource _source;
    private int IndexDeathFrame { get; set; }

    public BitmapSource Source
    {
        get => _source;
        private set => this.RaiseAndSetIfChanged(ref _source, value);
    }
    
    public BulletViewModel(Frames frames, Bullet bullet)
    {
        IndexDeathFrame = 0;
        Bullet = bullet;
        _source = frames.GetCurrent(bullet.BulletState);
        Frames = frames;
    }
    
    public void MoveNextFrame()
    {
        if (IndexDeathFrame == 23) return;
        var nextFrame = Frames.GetCurrent(Bullet.BulletState);
        if (Bullet.BulletState == EntityState.Death)
            IndexDeathFrame++;
        
        Source = nextFrame;
    }
}
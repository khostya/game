using System.Windows.Media.Imaging;
using ReactiveUI;

namespace WpfGame.Models;

public class ReactiveImageSource : ReactiveObject
{
    private BitmapSource _source;

    public BitmapSource Source
    {
        get => _source;
        set => this.RaiseAndSetIfChanged(ref _source, value);
    }

    public ReactiveImageSource(BitmapSource source) => _source = source;
}
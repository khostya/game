using System;
using ReactiveUI;
using Splat;

namespace WpfGame.ViewModels;

public abstract class RoutingViewModelBase : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment { get; }
    private Lazy<IScreen> LazyHostScreen { get; }

    public IScreen HostScreen => LazyHostScreen.Value;

    protected RoutingViewModelBase(IScreen? screen = null)
    {
        UrlPathSegment = Guid.NewGuid().ToString();
        LazyHostScreen = new Lazy<IScreen>(() => screen ?? Locator.Current.GetService<IScreen>()!);
    }
}
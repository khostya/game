using System.Reflection;
using System.Windows;
using ReactiveUI;
using Splat;
using WpfGame.Models;

namespace WpfGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var settings = new Settings();
            Locator.CurrentMutable.Register(() => settings);
            Locator.CurrentMutable.Register(() => new Size(40, 40));
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
        }
    }
}
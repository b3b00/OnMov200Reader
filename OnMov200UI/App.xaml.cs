using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using onmov200.Services;
using OnMov200UI.Views;
using OnMov200UI.ViewModels;

namespace OnMov200UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var db = new Database("./");

                var context = new MainWindowViewModel(db);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = context
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
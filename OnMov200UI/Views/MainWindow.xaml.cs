using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;


namespace OnMov200UI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
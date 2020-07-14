using System;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using onmov200.Models;
using ReactiveUI;

namespace OnMov200UI.Views
{
    public class NoDeviceView : UserControl
    {
        public NoDeviceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
using System;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using onmov200.Models;
using ReactiveUI;

namespace OnMov200UI.Views
{
    public class ActivityListView : UserControl
    {


        
        public ActivityListView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
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

        public ReactiveCommand<TodoItem,Unit> CheckThis { get; }

        
        public ActivityListView()
        {
            CheckThis = ReactiveCommand.Create(
                (TodoItem item) => {
                    Console.WriteLine($"{item.Description} - {item.IsChecked}");
                    });
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
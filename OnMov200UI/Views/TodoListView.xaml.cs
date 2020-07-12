using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using avaTodo.Models;
using System.Reactive;
using System;

namespace avaTodo.Views
{
    public class TodoListView : UserControl
    {

        public ReactiveCommand<TodoItem,Unit> CheckThis { get; }

        
        public TodoListView()
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
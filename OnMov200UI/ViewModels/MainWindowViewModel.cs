using System;
using System.Reactive.Linq;
using ReactiveUI;
using avaTodo.Models;
using avaTodo.Services;

namespace avaTodo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
  ViewModelBase content;

        public MainWindowViewModel(Database db)
        {
            Content = List = new TodoListViewModel(db.GetItems());
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public TodoListViewModel List { get; }

        
    }
}

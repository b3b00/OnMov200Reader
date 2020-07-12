using System.Collections.Generic;
using System.Collections.ObjectModel;
using avaTodo.Models;

namespace avaTodo.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public TodoListViewModel(IEnumerable<ActivityModel> items)
        {
            Items = new ObservableCollection<ActivityModel>(items);
        }

        public ObservableCollection<ActivityModel> Items { get; }
    }
}
using System.Linq;
using onmov200.Models;
using onmov200.Services;
using ReactiveUI;

namespace OnMov200UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        private Database Database;

        public MainWindowViewModel(Database db)
        {
            Database = db;
            Content = new ActivityListViewModel(db.GetActivities());
        }

        public ViewModelBase Content
        {
            get => content;
            private set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }

        public ActivityListViewModel Activities => Content as ActivityListViewModel;

        public void ExtractActivities()
        {
            var toExtract = Activities.Activities.Where(x => x.Checked).ToList();
            foreach (var activityModel in toExtract)
            {
                Database.OnMov200.ExtractActivity(activityModel.Activity);
            }

        }
        
        public void SelectAll()
        {
            var NewActivities = Activities.Activities.Select(x => new ActivityModel(x.Activity) {Checked = true});

            Content = new ActivityListViewModel(NewActivities);
        }
        
        public void UnSelectAll()
        {
            var NewActivities = Activities.Activities.Select(x => new ActivityModel(x.Activity) {Checked = false});

            Content = new ActivityListViewModel(NewActivities);
        }
    }
}

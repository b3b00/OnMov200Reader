using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using onmov200.Models;

namespace OnMov200UI.ViewModels
{
    public class ActivityListViewModel : ViewModelBase
    {
        public ActivityListViewModel(IEnumerable<ActivityModel> activities)
        {
            Activities = new ObservableCollection<ActivityModel>(activities);
        }

        public bool HasSelectedActivities => Activities.Any(x => x.Checked) ;

        public ObservableCollection<ActivityModel> Activities { get; }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is ActivityListViewModel list)
                {
                    if (list.Activities.Count == Activities.Count)
                    {
                        foreach (var activity in Activities)
                        {
                            var otherActivity = list.Activities.FirstOrDefault(x => x.Activity.Name == activity.Activity.Name);
                            if (otherActivity == null)
                            {
                                return false;
                            }
                            if (otherActivity.Checked != activity.Checked || otherActivity.Dirty != activity.Dirty)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

    }
}
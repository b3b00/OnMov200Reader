﻿using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using onmov200;
using onmov200.Models;
using onmov200.Services;
using OnMov200UI.Views;
using ReactiveUI;

namespace OnMov200UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        private Database Database;

        private OnMov200 OnMov200 => Database.OnMov200;

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

        public async Task ExtractActivities(object w)
        {
            var toExtract = Activities.Activities.Where(x => x.Checked).ToList();
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                DefaultDirectory = OnMov200.OutputDirectory
            };
            var folder = await  openFolderDialog.ShowAsync(w as MainWindow);
            foreach (var activityModel in toExtract)
            { 
                OnMov200.ExtractActivity(activityModel.Activity, folder);
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
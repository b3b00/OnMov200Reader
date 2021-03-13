using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using onmov200;
using onmov200.Models;
using onmov200.Services;
using OnMov200UI.Views;
using ReactiveUI;

using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace OnMov200UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private const string FastFixTitle = "FastFix";
        private const string ConfirmFastFixUpdate = "êtes vous sûr de vouloir forcer la mise à jour du FastFix ? ";
        private const string UpdatedFastFix = "Les données du FastFix ont été mises à jour.";
        private const string ExtractedData = "Les activités ont été extraites avec succés.";
        private const string ExtractionTitle = "Extraction";
        private const string ErrorTitle = "Erreurs";
        
        ViewModelBase content;

        private Database Database;

        private OnMov200 OnMov200 => Database.OnMov200;

        public MainWindowViewModel(Database db)
        {
            var cmdLine = Environment.GetCommandLineArgs();
            string rootDirectory = null;
            if (cmdLine.Length == 2)
            {
                if (Directory.Exists(cmdLine[1]))
                {
                    rootDirectory = cmdLine[1];
                }
            }
            
            Database = db;
            string root =  db.OnMov200.Detect(6,rootDirectory);
            if (root != null) {
                db.OnMov200.Initialize(root);
                Content = new ActivityListViewModel(db.GetActivities());
            }
            else {
                Content = new NoDeviceViewModel();
            }
        }

        public ViewModelBase Content
        {
            get => content;
            private set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }

        public void Update()
        {
            this.RaisePropertyChanged("FastFixLabel");
        }

        public ActivityListViewModel Activities => Content as ActivityListViewModel;

        public string FastFixLabel
        {
            get
            {
                var need = OnMov200.NeedFastFixUpdate(); 
                if (need.needUpdate)
                {
                    return $"Mise à jour du FastFix nécessaire - {need.age} jours";
                }
                else
                {
                    return $"Fastfix OK - {need.age} jours";
                }
            }
        }

        public bool NeedFastFix => OnMov200.NeedFastFixUpdate().needUpdate;

        public async Task ExtractActivities(object window)
        {
            var toExtract = Activities.Activities.Where(x => x.Checked).ToList();
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Directory = OnMov200.OutputDirectory
            };
            var folder = await  openFolderDialog.ShowAsync(window as MainWindow);

            var result = toExtract.Select(activityModel => OnMov200.ExtractActivity(activityModel.Activity, folder));
            
            var errors = result.Where(x => x.IsRight).Select(x => x.IfLeft(new OMError(null,"no error"))).ToList();
            if (errors.Count == 0)
            {
                await MessageBoxManager
                    .GetMessageBoxStandardWindow(ExtractionTitle, ExtractedData, ButtonEnum.Ok, Icon.Info).Show();
            }
            else
            {
                var errorsMessage = string.Join('\n',errors.Select(x =>x.ErrorMessage));
                await MessageBoxManager
                    .GetMessageBoxStandardWindow(ErrorTitle, errorsMessage, ButtonEnum.Ok, Icon.Error).Show();
            }
          
        }

        public async Task UpdateFastFix()
        {
            if (NeedFastFix)
            {
                await OnMov200.UpDateFastFixIfNeeded();
                await MessageBoxManager
                    .GetMessageBoxStandardWindow(FastFixTitle, UpdatedFastFix, ButtonEnum.Ok, Icon.Info).Show();
            }
            else
            {
                var answer = await MessageBoxManager
                    .GetMessageBoxStandardWindow(FastFixTitle, ConfirmFastFixUpdate, ButtonEnum.YesNo, Icon.Warning).Show();
                if (answer == ButtonResult.Yes)
                {
                    await OnMov200.UpDateFastFixIfNeeded(true);                   
                    await MessageBoxManager
                        .GetMessageBoxStandardWindow(FastFixTitle, UpdatedFastFix, ButtonEnum.Ok, Icon.None).Show();
                    Update();
                }
                    
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

        public void ChangeSelection(string name) {
            var NewActivities = Activities.Activities.Select(x => new ActivityModel(x.Activity) {Checked = x.Checked, Dirty = x.Activity.Name == name ^ x.Dirty});

            Content = new ActivityListViewModel(NewActivities);
        }

        public void DetectWatch() {
            string root =  Database.OnMov200.Detect(6);
            if (root != null) {
                Database.OnMov200.Initialize(root);
                Content = new ActivityListViewModel(Database.GetActivities());
            }
            else {
                Content = new NoDeviceViewModel();
            }
        }

        public override bool Equals(object obj)
        {
            return false;
        }
    }
}

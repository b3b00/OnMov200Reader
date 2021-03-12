using System.Linq;
using System.Threading.Tasks;
using Avalonia;
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
        ViewModelBase content;

        private Database Database;

        private OnMov200 OnMov200 => Database.OnMov200;

        public MainWindowViewModel(Database db)
        {
            Database = db;
            string root =  db.OnMov200.Detect(6);
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

        public async Task ExtractActivities(object w)
        {
            var toExtract = Activities.Activities.Where(x => x.Checked).ToList();
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Directory = OnMov200.OutputDirectory
            };
            var folder = await  openFolderDialog.ShowAsync(w as MainWindow);

            var result = toExtract.Select(activityModel => OnMov200.ExtractActivity(activityModel.Activity, folder));
            
            var errors = result.Where(x => x.IsRight).Select(x => x.IfLeft(new OMError(null,"no error"))).ToList();
            if (errors.Count == 0)
            {
                await MessageBoxManager
                    .GetMessageBoxStandardWindow("extraction", "Les activités ont été extraites avec succés.", ButtonEnum.Ok, Icon.Info).Show();
            }
            else
            {
                var errorsMessage = string.Join('\n',errors.Select(x =>x.ErrorMessage));
                await MessageBoxManager
                    .GetMessageBoxStandardWindow("Erreurs", errorsMessage, ButtonEnum.Ok, Icon.Error).Show();
            }
          
        }

        public async Task UpdateFastFix()
        {
            if (NeedFastFix)
            {
                await OnMov200.UpDateFastFixIfNeeded();
            }
            else
            {
                var answer = await MessageBoxManager
                    .GetMessageBoxStandardWindow("mise à jour du FastFix", "êtes vous sûr de vouloir forcer la mise à jour du FastFix ? ", ButtonEnum.YesNo, Icon.Warning).Show();
                if (answer == ButtonResult.Yes)
                {
                    await OnMov200.UpDateFastFixIfNeeded(true);
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

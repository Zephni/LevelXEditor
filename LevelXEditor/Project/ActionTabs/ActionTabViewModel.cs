using LevelXEditor.Project.Views;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelXEditor.Project.ActionTabs
{
    /// view model for the TabControl To bind on
    public class ActionTabViewModal : ViewModelBase
    {
        // These Are the tabs that will be bound to the TabControl
        private ObservableCollection<ActionTabItem> tabs;
        public ObservableCollection<ActionTabItem> Tabs {
            get { return tabs; } set { tabs = value; NotifyPropertyChanged("Tabs"); }
        }

        public ActionTabViewModal()
        {
            Tabs = new ObservableCollection<ActionTabItem>();
            tabs = Tabs;
        }
    }
}

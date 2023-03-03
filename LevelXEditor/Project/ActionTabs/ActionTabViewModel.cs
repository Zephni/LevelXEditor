using LevelXEditor.Project.Views;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LevelXEditor.Project.ActionTabs
{
    /// view model for the TabControl To bind on
    public class ActionTabViewModal : ViewModelBase
    {
        // TabControl to bind to
        public TabControl tabControl { get; private set; }

        // These Are the tabs that will be bound to the TabControl
        private ObservableCollection<ActionTabItem> tabs;
        public ObservableCollection<ActionTabItem> Tabs {
            get { return tabs; } set { tabs = value; NotifyPropertyChanged("Tabs"); }
        }

        public ActionTabViewModal(TabControl _tabControl)
        {
            Tabs = new ObservableCollection<ActionTabItem>();
            tabs = Tabs;

            this.tabControl = _tabControl;
        }

        public void AddTab(UserControl userControl, int? index = null)
        {
            // Create new tab
            ActionTabItem actionTabItem = new(userControl);

            // Add tab to view model
            if (index == null) Tabs.Add(actionTabItem);
            else Tabs.Insert((int)index, actionTabItem);

            // Select tab
            tabControl.SelectedItem = actionTabItem;
        }

        public void CloseTab(int index)
        {
            // Give focus
            tabControl.Focus();

            // If we are closing the last tab, move to the previous tab
            if (index == tabControl.Items.Count - 1) tabControl.SelectedIndex = index - 1;
            
            // If we are closing the first tab, move to the next tab
            else if (index == 0) tabControl.SelectedIndex = index + 1;
            
            // If we are closing a tab in the middle, move to the previous tab
            else tabControl.SelectedIndex = index - 1;

            // Remove tab from view model (With small delay)
            SubRoutines.Wait(0.1f, () => {
                Tabs.RemoveAt(index);
            });
        }
    }
}

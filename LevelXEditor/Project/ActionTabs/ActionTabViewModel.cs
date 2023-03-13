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
            tabs = Tabs; // Fix for null reference exception

            tabControl = _tabControl;
            tabControl.ItemsSource = Tabs;
        }

        public void UpdateTabHeaders()
        {
            foreach (ActionTabItem tab in Tabs) tab.SetHeaderFromUserControlTag();
        }

        public void PropertiesChanged()
        {
            NotifyPropertyChanged("Tabs");
            foreach(ActionTabItem tab in Tabs) tab.PropertiesChanged();
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

        public ActionTabItem? GetTabItem(UserControl userControl)
        {
            foreach (ActionTabItem tab in Tabs)
            {
                if (tab.UserControl == userControl) return tab;
            }

            return null;
        }

        public ActionTabItem? GetTabItem(string userControlClassName)
        {
            foreach (ActionTabItem tab in Tabs)
            {
                // Get the class name from the tab's UserControl
                string className = tab.UserControl?.GetType().Name ?? "";
                if (className == userControlClassName) return tab;
            }

            return null;
        }

        public ActionTabItem GetTabWhere(Func<ActionTabItem, bool> predicate)
        {
            foreach (ActionTabItem tab in Tabs)
            {
                if (predicate(tab)) return tab;
            }

            return null;
        }

        public List<ActionTabItem> GetTabsWhere(Func<ActionTabItem, bool> predicate)
        {
            List<ActionTabItem> tabs = new();
            foreach (ActionTabItem tab in Tabs)
            {
                if (predicate(tab)) tabs.Add(tab);
            }

            return tabs;
        }

        public bool IsTabOpen(string userControlClassName)
        {
            return GetTabItem(userControlClassName) != null;
        }

        public void SwitchToTab(ActionTabItem actionTabItem)
        {
            tabControl.SelectedItem = actionTabItem;
        }

        public void SwitchToTab(string userControlClassName)
        {
            ActionTabItem? tab = GetTabItem(userControlClassName);
            if (tab != null) tabControl.SelectedItem = tab;
        }

        public void SwitchToTabWhere(Func<ActionTabItem, bool> predicate)
        {
            ActionTabItem? tab = GetTabWhere(predicate);
            if (tab != null) tabControl.SelectedItem = tab;
        }
    }
}

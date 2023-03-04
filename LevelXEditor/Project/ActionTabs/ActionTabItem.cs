using LevelXEditor.Project.Views;
using MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LevelXEditor.Project.ActionTabs
{
    // This class will be the Tab int the TabControl
    public class ActionTabItem : ViewModelBase
    {
        public UserControl? UserControl { get; set; }

        private string header = "No Name";
        public string Header { get { return header; } set { header = value; NotifyPropertyChanged("Header"); } }

        public ActionTabItem(UserControl userControl)
        {
            UserControl = userControl;
            SetHeaderFromUserControlTag();
        }

        public void SetHeaderFromUserControlTag()
        {
            Header = (string)UserControl.Tag;
        }

        public void PropertiesChanged()
        {
            NotifyPropertyChanged("Header");
        }
    }
}

using LevelXEditor.Project.Views;
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
    public class ActionTabItem
    {
        public UserControl? UserControl { get; set; }

        public string Header { 
            get
            {
                return UserControl.Tag?.ToString() ?? "No Name";
            }
        }

        public ActionTabItem(UserControl userControl)
        {
            UserControl = userControl;
        }
    }
}

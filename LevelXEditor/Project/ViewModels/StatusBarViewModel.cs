using LevelXEditor.Project.Views;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LevelXEditor.Project.StatusBar
{
    /// view model for the Status Bar to bind on
    public class StatusBarViewModal : ViewModelBase
    {
        private string statusLastMessage = "";
        public string StatusLastMessage { get => statusLastMessage; set { statusLastMessage = value; NotifyPropertyChanged("StatusLastMessage"); } }

        private string statusHoverContext = "";
        public string StatusHoverContext { get => statusHoverContext; set { statusHoverContext = value; NotifyPropertyChanged("StatusHoverContext"); } }

        // Constructor
        public StatusBarViewModal()
        {
            // Set default status message
            StatusLastMessage = "Application started successfully";
            StatusHoverContext = "";
        }
    }
}
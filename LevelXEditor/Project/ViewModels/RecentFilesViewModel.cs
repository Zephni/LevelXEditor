using LevelXEditor.Project.Views;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LevelXEditor.Project.RecentFiles
{
    public class RecentFilesViewModel : ViewModelBase
    {
        public static RecentFilesViewModel instance;

        public struct RecentFile
        {
            public string FilePath { get; set; }
            public bool IsEnabled { get; set; }
        }

        private ObservableCollection<RecentFile> recentFiles = new();
        public ObservableCollection<RecentFile> RecentFiles { get => recentFiles; set { recentFiles = value; NotifyPropertyChanged("RecentFiles"); } }

        // Constructor
        public RecentFilesViewModel()
        {
            instance = this;
            RecentFiles = new ObservableCollection<RecentFile>();
            Refresh();
        }

        public void Refresh()
        {
            // Clear recent files
            RecentFiles.Clear();

            // Add recent files
            foreach (string recentFile in MainWindow.AppDataHandler.Data.recentFiles)
            {
                RecentFiles.Add(new RecentFile() { FilePath = recentFile, IsEnabled = true });
            }

            // If there are no recent files then add a placeholder
            if (RecentFiles.Count == 0)
            {
                RecentFiles.Add(new RecentFile() { FilePath = "No recent files", IsEnabled = false });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace LevelXEditor.Project.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();

            StreamResourceInfo info = Application.GetResourceStream(new Uri("Resources/HTML/Dashboard.html", UriKind.Relative));
            string html = new StreamReader(info.Stream).ReadToEnd();
            webBrowser.ObjectForScripting = new ScriptingObject();

            // Search the html with regex to find "__RESOURCE/Any/Resource/Path.[png|jpg|gif]" where __RESOURCE is a special identifier
            // and /Any/Resource/Path.png can be any path to a resource in the project
            Regex regex = new Regex(@"__RESOURCE\/.*?\.(png|jpg|gif)");
            MatchCollection matches = regex.Matches(html);
            foreach (Match match in matches) {
                string resourcePath = match.Value.Replace("__RESOURCE/", "");
                StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(resourcePath, UriKind.Relative));
                string base64 = Convert.ToBase64String(new BinaryReader(resourceInfo.Stream).ReadBytes((int)resourceInfo.Stream.Length));
                html = html.Replace(match.Value, "data:image/png;base64," + base64);
            }

            // Inject the recent files into the html
            html = InjectRecentFiles(html);

            webBrowser.NavigateToString(html);
        }

        private string ReplaceHTML(string replace, string with, string html)
        {
            return html.Replace(replace, with);
        }

        private string InjectRecentFiles(string html)
        {
            // For now create a dummy set of recent files
            List<string> recentFiles = new List<string>();
            recentFiles.Add("C:\\Users\\Documents\\LevelX\\Level1.lvlx");
            recentFiles.Add("C:\\Users\\Documents\\LevelX\\Level2.lvlx");
            recentFiles.Add("C:\\Users\\Documents\\LevelX\\Level3.lvlx");
            recentFiles.Add("C:\\Users\\Documents\\LevelX\\Level4.lvlx");

            // Create the html for the recent files
            string recentFilesHTML = "";
            foreach (string recentFile in recentFiles) {
                // Using bootstrap 4
                recentFilesHTML += @"
                    <div class='d-block'>
                        <a href='#' class='text-primary' onclick='window.external.MenuItem_File_Button('MenuItem_File_Open');'>
                            <i class='bi bi-file-earmark-text text-primary'></i>
                            " + recentFile + @"
                        </a>
                    </div>
                ";
            }

            // Inject the recent files html into the html
            return ReplaceHTML("<!-- __RECENT_FILES -->", recentFilesHTML, html);
        }
    }

    [ComVisible(true)]
    public class ScriptingObject {
        public void MenuItem_File_Button(string name, string tag = "") {
            // We can use tag as a way to pass custom parameters to the MenuItem_File_Button method
            MainWindow.instance.MenuItem_File_Button(new MenuItem {Name = name, Tag = tag}, new RoutedEventArgs());
        }
    }
}

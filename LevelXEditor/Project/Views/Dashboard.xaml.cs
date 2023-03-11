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
            RenderPage();
        }

        public void RenderPage()
        {
            string html = File.ReadAllText("Resources/HTML/Dashboard.html");
            webBrowser.ObjectForScripting = new ScriptingObject();

            // Search the html with regex to find "__RESOURCE/Any/Resource/Path.[png|jpg|gif]" where __RESOURCE is a special identifier
            // and /Any/Resource/Path.png can be any path to a resource in the project
            Regex regex = new Regex(@"__RESOURCE\/.*?\.(png|jpg|gif)");
            MatchCollection matches = regex.Matches(html);
            foreach (Match match in matches) {
                string resourcePath = match.Value.Replace("__RESOURCE/", "");
                String base64 = new BitmapImage(new Uri(resourcePath, UriKind.Relative)).ToBase64String();
                html = html.Replace(match.Value, "data:image/png;base64," + base64);
            }

            // Inject the recent files into the html and navigate to html string
            html = InjectRecentFiles(html);
            webBrowser.NavigateToString(html);
            
        }

        private string ReplaceHTML(string replace, string with, string html)
        {
            return html.Replace(replace, with);
        }

        private string InjectRecentFiles(string html)
        {
            // Create the html for the recent files
            string recentFilesHTML = "";

            foreach (string recentFile in MainWindow.AppDataHandler.Data.recentFiles) {
                string escapedfile = recentFile.Replace(@"\", @"\\");
                escapedfile = escapedfile.Replace(@"\\\", @"\\");
                
                recentFilesHTML += @"
                    <div class='d-block'>
                        <a href='#' title='" + escapedfile + @"' class='text-primary' onclick='window.external.MenuItem_File_Button(""File_Open"", """ + escapedfile + @""");'>
                            <i class='bi bi-file-earmark-text text-primary'></i>
                            " + recentFile + @"
                        </a>
                    </div>
                ";
            }

            // Inject the recent files html into the html
            return ReplaceHTML("<!-- __RECENT_FILES -->", recentFilesHTML, html);
        }

        public static void Refresh()
        {
            if(MainWindow.instance.actionTabsModel.GetTabItem("Dashboard")?.UserControl == null)
            {
                return;
            }

            SubRoutines.Wait(0.1f, () => {
                Dashboard? dashboard = (Dashboard?)MainWindow.instance.actionTabsModel.GetTabItem("Dashboard")?.UserControl;
                dashboard?.RenderPage();
            });
        }
    }

    [ComVisible(true)]
    public class ScriptingObject {
        public void MenuItem_File_Button(string name, string parameter = "") {
            // We can use tag as a way to pass custom parameters to the appropriate AppMenu method
            MainWindow.instance.AppMenu.CallAssociatedMethod(new MenuItem { Tag = name, ToolTip = parameter }, new RoutedEventArgs());
        }
    }
}

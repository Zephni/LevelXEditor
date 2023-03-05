using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace LevelXEditor
{
    public static class Utilities
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        // Allow any number of parameters to drill down to a specific menu item by their names, eg "File", "Save":
        // And then find the element like so, but dynamically using the parameters: (MenuItem)applicationMenu.Items.Cast<MenuItem>().Where(x => x.Header.ToString() == "_File").First().Items.Cast<MenuItem>().Where(x => x.Header.ToString() == "_Save").First()
        public static MenuItem GetApplicationMenuItem(params string[] names)
        {
            // Get the application menu
            Menu applicationMenu = (Menu)MainWindow.instance.ApplicationMenu;

            // Get the first menu item from the application menu
            MenuItem menuItem = (MenuItem)applicationMenu.Items.Cast<MenuItem>().Where(x => x.Header.ToString() == names[0]).First();

            for(int i = 1; i < names.Length; i++)
            {
                // If we are searching for a  sub item
                menuItem = (MenuItem)menuItem.Items.Cast<MenuItem>().Where(x => x.Header.ToString() == names[i]).First();
            }

            return menuItem;
        }
    }
}

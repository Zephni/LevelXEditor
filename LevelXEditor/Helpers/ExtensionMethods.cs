using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LevelXEditor
{
    public static class ExtensionMethods
    {
        // T Modify | Modifies an object and returns it
        public static T Modify<T>(this T obj, Action<T> modifier)
        {
            modifier(obj);
            return obj;
        }

        // ScaleTransform SetScale | Sets ScaleX and ScaleY by a Point
        public static void SetScale(this ScaleTransform scaleTransform, Point scale)
        {
            scaleTransform.ScaleX = scale.X;
            scaleTransform.ScaleY = scale.Y;
        }

        // ScrollViewer ScrollToOffset | Scrolls to a Point
        public static void ScrollToOffset(this ScrollViewer scrollViewer, Point offset)
        {
            scrollViewer.ScrollToHorizontalOffset(offset.X);
            scrollViewer.ScrollToVerticalOffset(offset.Y);
        }

        // ScrollViewer ScrollToCenter | Scrolls to the center of the ScrollViewer
        public static void ScrollToCenter(this ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToOffset(new Point(scrollViewer.ScrollableWidth / 2, scrollViewer.ScrollableHeight / 2));
        }

        // ScrollViewer GetScrollOffset | Gets the current scroll offset
        public static Point GetScrollOffset(this ScrollViewer scrollViewer)
        {
            return new Point(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
        }

        // ContentPresenter FindContentByName | Finds content by name from this ContentPresenter's DataTemplate
        public static T FindContentByName<T>(this ContentPresenter contentPresenter, string name)
        {
            // Throw exception if ContentTemplate is null
            if (contentPresenter.ContentTemplate == null)
            {
                throw new Exception("ContentPresenter's DataTemplate is null");
            }

            // Find element
            T element = (T)contentPresenter.ContentTemplate.FindName(name, contentPresenter);

            // Throw exception if element is null
            if (element == null)
            {
                throw new Exception($"Element with name {name} not found in ContentPresenter's DataTemplate");
            }

            return element;
        }

        // DependencyObject FindChildByType | Finds a child of a control by type
        public static T FindChildOfType<T>(this DependencyObject depObj, Func<T, bool> condition = null) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                if (child != null && child is T)
                {
                    if (condition != null)
                    {
                        if (condition((T)child))
                        {
                            return (T)child;
                        }
                    }
                    else
                    {
                        return (T)child;
                    }
                }
            }

            return null;
        }

        // DependencyObject GetContentPresenter | Gets the ContentPresenter of a control
        public static ContentPresenter GetContentPresenter(this DependencyObject depObj)
        {
            return Utilities.FindVisualChild<ContentPresenter>(depObj);
        }

        // Selector GetItemContainer | Gets the container of a control
        public static DependencyObject GetItemContainer(this Selector selector, int? index = null)
        {
            return selector.ItemContainerGenerator.ContainerFromIndex(index ?? selector.SelectedIndex);
        }

        // BitmapImage ToBase64String | Converts a BitmapImage to a Base64 string
        public static string ToBase64String(this BitmapImage bitmapImage)
        {
            // Create a bitmap encoder
            BitmapEncoder encoder = new PngBitmapEncoder();

            // Add the bitmap frame to the encoder
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            // Create a memory stream
            using System.IO.MemoryStream memoryStream = new();

            // Encode the bitmap to the memory stream
            encoder.Save(memoryStream);

            // Return the base64 string
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}

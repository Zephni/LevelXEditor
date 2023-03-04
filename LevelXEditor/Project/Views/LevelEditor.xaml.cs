using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LevelXEditor.Project.Views
{
    /// <summary>
    /// Interaction logic for LevelEditor.xaml
    /// </summary>
    public partial class LevelEditor : UserControl
    {
        private Point _lastMousePosition;
        private Point defaultScaleSize = new(1.75, 1.75);
        public LevelDataHandler LevelDataHandler { get; set; } = new();

        public LevelEditor()
        {
            InitializeComponent();
            
            levelScrollViewer.MouseDown += OnMouseDown;
            levelScrollViewer.MouseMove += OnMouseMove;
            levelScrollViewer.PreviewMouseWheel += OnMouseWheel;

            // Register MainWindow key down events
            MainWindow.RegisterKeyDownEvent((sender, e) => {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.D0)
                {
                    levelScaleTransform.SetScale(defaultScaleSize);
                }
            });

            // Set zoom to default scale size
            levelScaleTransform.SetScale(defaultScaleSize);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _lastMousePosition = e.GetPosition(levelScrollViewer);
                levelScrollViewer.CaptureMouse();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (levelScrollViewer.IsMouseCaptured && e.MiddleButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(levelScrollViewer);
                Point delta = new(position.X - _lastMousePosition.X, position.Y - _lastMousePosition.Y);
                Point finalOffset = new(levelScrollViewer.HorizontalOffset - delta.X, levelScrollViewer.VerticalOffset - delta.Y);
                levelScrollViewer.ScrollToOffset(finalOffset);

                _lastMousePosition = position;
            }
            else
            {
                levelScrollViewer.ReleaseMouseCapture();
            }
        }
        
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {            
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double scaleDelta = e.Delta > 0 ? 0.1 : -0.1;
                double scaleX = levelScaleTransform.ScaleX + scaleDelta;
                double scaleY = levelScaleTransform.ScaleY + scaleDelta;
                scaleX = Math.Max(0.1, Math.Min(10, scaleX));
                scaleY = Math.Max(0.1, Math.Min(10, scaleY));
                levelScaleTransform.ScaleX = scaleX;
                levelScaleTransform.ScaleY = scaleY;
                e.Handled = true;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ZoomHandler(e.Delta > 0 ? 0.1 : -0.1);
                e.Handled = true;
            }
        }

        private void ZoomHandler(double scaleDelta)
        {
            double scaleX = levelScaleTransform.ScaleX + scaleDelta;
            double scaleY = levelScaleTransform.ScaleY + scaleDelta;
            scaleX = Math.Max(0.1, Math.Min(10, scaleX)); // set minimum and maximum values
            scaleY = Math.Max(0.1, Math.Min(10, scaleY)); // set minimum and maximum values
            levelScaleTransform.ScaleX = scaleX;
            levelScaleTransform.ScaleY = scaleY;
        }
    }
}

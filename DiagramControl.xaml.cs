using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DiagramControl.Views
{
    public partial class DiagramControl : UserControl
    {
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        private TransformGroup _transformGroup;
        private bool _isPanning = false;
        private Point _panStartPoint;
        private double _zoomLevel = 1.0;
        private const double ZoomIncrement = 0.1;
        private const double MinZoom = 0.1;
        private const double MaxZoom = 5.0;

        public DiagramControl()
        {
            InitializeComponent();
            InitializeTransforms();
            InitializeSampleDiagram();
        }

        private void InitializeTransforms()
        {
            _scaleTransform = new ScaleTransform(1.0, 1.0);
            _translateTransform = new TranslateTransform(0, 0);
            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);
            DiagramCanvas.RenderTransform = _transformGroup;
            DiagramCanvas.RenderTransformOrigin = new Point(0, 0);
        }

        private void InitializeSampleDiagram()
        {
            // Add sample shapes to demonstrate the diagram
            AddRectangle(100, 100, 150, 80, "Process 1", Brushes.LightBlue);
            AddRectangle(350, 100, 150, 80, "Process 2", Brushes.LightGreen);
            AddRectangle(600, 100, 150, 80, "Process 3", Brushes.LightCoral);
            
            AddEllipse(225, 200, 100, 100, "Decision", Brushes.LightYellow);
            AddRectangle(150, 350, 150, 80, "Output", Brushes.LightGray);

            // Draw connecting lines
            DrawLine(175, 180, 275, 200);
            DrawLine(425, 180, 275, 250);
            DrawLine(675, 180, 275, 250);
            DrawLine(225, 300, 225, 350);
        }

        private void AddRectangle(double x, double y, double width, double height, string label, Brush fill)
        {
            var rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            DiagramCanvas.Children.Add(rect);

            var text = new TextBlock
            {
                Text = label,
                Foreground = Brushes.Black,
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(text, x + 10);
            Canvas.SetTop(text, y + height / 2 - 10);
            DiagramCanvas.Children.Add(text);
        }

        private void AddEllipse(double x, double y, double width, double height, string label, Brush fill)
        {
            var ellipse = new Ellipse
            {
                Width = width,
                Height = height,
                Fill = fill,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            DiagramCanvas.Children.Add(ellipse);

            var text = new TextBlock
            {
                Text = label,
                Foreground = Brushes.Black,
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(text, x + width / 2 - 30);
            Canvas.SetTop(text, y + height / 2 - 10);
            DiagramCanvas.Children.Add(text);
        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            DiagramCanvas.Children.Add(line);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Zoom(_zoomLevel + ZoomIncrement);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Zoom(_zoomLevel - ZoomIncrement);
        }

        private void Zoom(double newZoom)
        {
            if (newZoom < MinZoom || newZoom > MaxZoom)
                return;

            _zoomLevel = newZoom;
            _scaleTransform.ScaleX = _zoomLevel;
            _scaleTransform.ScaleY = _zoomLevel;
            UpdateZoomLevel();
        }

        private void FitToView_Click(object sender, RoutedEventArgs e)
        {
            // Calculate the bounding box of all elements
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (UIElement element in DiagramCanvas.Children)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                double width = (element as FrameworkElement)?.Width ?? 0;
                double height = (element as FrameworkElement)?.Height ?? 0;

                if (left < minX) minX = left;
                if (top < minY) minY = top;
                if (left + width > maxX) maxX = left + width;
                if (top + height > maxY) maxY = top + height;
            }

            if (minX == double.MaxValue) return;

            double boundingWidth = maxX - minX;
            double boundingHeight = maxY - minY;

            double zoomX = (ScrollViewer.ViewportWidth - 20) / boundingWidth;
            double zoomY = (ScrollViewer.ViewportHeight - 20) / boundingHeight;
            _zoomLevel = Math.Min(zoomX, zoomY);

            _zoomLevel = Math.Max(MinZoom, Math.Min(MaxZoom, _zoomLevel));
            _scaleTransform.ScaleX = _zoomLevel;
            _scaleTransform.ScaleY = _zoomLevel;

            UpdateZoomLevel();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel = 1.0;
            _scaleTransform.ScaleX = 1.0;
            _scaleTransform.ScaleY = 1.0;
            _translateTransform.X = 0;
            _translateTransform.Y = 0;
            ScrollViewer.ScrollToHome();
            UpdateZoomLevel();
            StatusTextBlock.Text = "View reset";
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1 && Keyboard.IsKeyDown(Key.Space))
            {
                _isPanning = true;
                _panStartPoint = e.GetPosition(ScrollViewer);
                DiagramCanvas.Cursor = Cursors.Hand;
                e.Handled = true;
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            DiagramCanvas.Cursor = Cursors.Arrow;
            e.Handled = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(DiagramCanvas);
            MousePositionTextBlock.Text = $"Position: ({position.X:F0}, {position.Y:F0})";

            if (_isPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(ScrollViewer);
                double offsetX = currentPoint.X - _panStartPoint.X;
                double offsetY = currentPoint.Y - _panStartPoint.Y;

                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - offsetX);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - offsetY);

                _panStartPoint = currentPoint;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double zoomDelta = e.Delta > 0 ? ZoomIncrement : -ZoomIncrement;
                Zoom(_zoomLevel + zoomDelta);
                e.Handled = true;
            }
        }

        private void UpdateZoomLevel()
        {
            ZoomLevelTextBlock.Text = $"{(_zoomLevel * 100):F0}%";
            StatusTextBlock.Text = $"Zoom: {(_zoomLevel * 100):F0}%";
        }

        // Public method to add custom shapes
        public void AddShape(UIElement shape, double x, double y)
        {
            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);
            DiagramCanvas.Children.Add(shape);
        }

        // Public method to clear the diagram
        public void ClearDiagram()
        {
            DiagramCanvas.Children.Clear();
        }

        // Public method to get current zoom level
        public double GetZoomLevel()
        {
            return _zoomLevel;
        }

        // Public method to set zoom level
        public void SetZoomLevel(double zoom)
        {
            Zoom(Math.Max(MinZoom, Math.Min(MaxZoom, zoom)));
        }
    }
}

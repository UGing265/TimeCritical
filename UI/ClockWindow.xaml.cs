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
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow : Window
    {
        private double aspectRatio = 4.0 / 3.0; // 4:3 aspect ratio

        public ClockWindow()
        {
            InitializeComponent();
            this.SizeChanged += ClockWindow_SizeChanged;
            this.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            this.SourceInitialized += (s, e) => ApplyRoundedCorners();
        }

        private void ApplyRoundedCorners()
        {
            // Create rounded rectangle geometry
            var rect = new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight), 30, 30);
            this.Clip = rect;
        }

        private void ClockWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Update clipping when resized
            ApplyRoundedCorners();

            // Maintain 4:3 aspect ratio - prioritize width
            double expectedHeight = this.Width / aspectRatio;
            if (Math.Abs(this.Height - expectedHeight) > 0.5)
            {
                this.Height = expectedHeight;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Allow dragging the window
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}

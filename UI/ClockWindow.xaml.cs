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
        private System.Windows.Threading.DispatcherTimer _timer;
        public DateTime TargetEndTime { get; set; }
        public bool IsCountingDown { get; set; } = false;
        public MainWindow ParentWindow { get; set; }

        public ClockWindow()
        {
            InitializeComponent();
            this.SizeChanged += ClockWindow_SizeChanged;
            this.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            this.SourceInitialized += (s, e) => ApplyRoundedCorners();
            
            // Setup Timer
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            
            BtnClose.Click += (s, e) => this.Close();
            BtnDone.Click += BtnDone_Click;
            BtnStopTimer.Click += BtnStopTimer_Click;
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

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!IsCountingDown)
            {
                TxtTime.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            else
            {
                TimeSpan remaining = TargetEndTime - DateTime.Now;

                if (remaining.TotalSeconds <= 0)
                {
                    _timer.Stop();
                    TxtTime.Text = "00:00:00";
                }
                else
                {
                    TxtTime.Text = $"{remaining:hh\\:mm\\:ss}";
                }
            }
        }

        public void StartCountdown(DateTime targetEndTime)
        {
            TargetEndTime = targetEndTime;
            IsCountingDown = true;
            _timer.Start();
        }

        public void StopCountdown()
        {
            IsCountingDown = false;
            _timer.Stop();
            TxtTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // --- BUTTON HANDLERS ---
        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Cancel shutdown timer?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CancelShutdown();
            }
        }

        private void BtnStopTimer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Cancel shutdown timer?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                CancelShutdown();
            }
        }

        private void CancelShutdown()
        {
            // Call MainWindow's cancel method
            if (ParentWindow != null)
            {
                ParentWindow.CancelShutdownTimer();
            }
        }    }
}
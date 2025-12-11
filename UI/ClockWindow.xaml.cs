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
        private bool isUserResizing = false;

        public ClockWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => 
            {
                if (e.OriginalSource == this)
                    isUserResizing = true;
            };
            this.MouseLeftButtonUp += (s, e) => isUserResizing = false;
            this.SizeChanged += ClockWindow_SizeChanged;
        }

        private void ClockWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Maintain 4:3 aspect ratio - prioritize width
            double expectedHeight = this.Width / aspectRatio;
            if (Math.Abs(this.Height - expectedHeight) > 0.5)
            {
                this.Height = expectedHeight;
            }
        }
    }
}

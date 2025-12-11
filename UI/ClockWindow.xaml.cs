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
        }

        private void ClockWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Maintain 4:3 aspect ratio
            if (e.WidthChanged)
            {
                this.Height = this.Width / aspectRatio;
            }
            else if (e.HeightChanged)
            {
                this.Width = this.Height * aspectRatio;
            }
        }
    }
}

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();




        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            // 1. Lấy Text từ UI
            string startText = TxtStartTime.Text;
            string endText = TxtEndTime.Text;

            // 2. Parse thời gian (định dạng HH:mm)
            if (!TimeSpan.TryParse(startText, out var start))
            {
                MessageBox.Show("Oh no! start time is invaid HH:mm (23:00).", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; //tránh spam thông báo lỗi ở dưới nữa

            }

            if (!TimeSpan.TryParse(endText, out var end))
            {
                MessageBox.Show("Oh no! end time is invaid! HH:mm (23:00).", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Lấy thời gian hiện tại (Theo hôm nay)
            DateTime now = DateTime.Now;
            DateTime today = now.Date;
            DateTime startAt = today + start;
            DateTime endAt = today + end;

            if (endAt < now)
            {
                MessageBox.Show($"Alo, Now is {now.ToString("HH:mm tt")} and End time is {endAt.ToString("HH:mm tt")}.\n It's invaid logic", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (startAt < now)
            {
                MessageBox.Show($"Alo, Now is {now.ToString("HH:mm tt")} and Start time is {startAt.ToString("HH:mm tt")}.\n It's invaid logic", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return ;
            }


            // 4. Tính thời gian từ bây giờ đến End
            TimeSpan delta = endAt - now;
            int totalSeconds = (int)delta.TotalSeconds;

            // 5. Hỏi Confirm
            string lockText = ChkLock.IsChecked == true ? "LOCK" : "SHUTDOWN";
            string message =
              $"Thời gian hiện tại: {now:HH:mm}\n" +
              $"End: {endAt:HH:mm}\n" +
              $"Còn lại: {delta:hh\\:mm\\:ss}\n" +
              $"Chế độ: {lockText}\n" +
              $"Xác nhận đặt lịch?";

            var result = MessageBox.Show(message,
                                         "Xác nhận đặt lịch",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                if (ChkLock.IsChecked == true)
                {
                    // ➜ Mode LOCK MÁY khi đến giờ End
                    // Dùng cmd + timeout để delay rồi lock:
                    // timeout /t <seconds> /nobreak && rundll32.exe user32.dll,LockWorkStation
                    //var psi = new ProcessStartInfo
                    //{
                    //    FileName = "cmd",
                    //    Arguments = $"/c timeout /t {totalSeconds} /nobreak && shutdown /s /f /t 0",
                    //    CreateNoWindow = true,
                    //    UseShellExecute = false
                    //};
                    //Process.Start(psi);
                    MessageBox.Show("Fake checked", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Fake no checkbox", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                    //Process.Start("shutdown", $"/s /t {totalSeconds}");
                }
                MessageBox.Show("Sucessful set schedule", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
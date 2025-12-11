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
        // --- 1. VARIABLES FOR THE COUNTDOWN ---
        System.Windows.Threading.DispatcherTimer _timer;
        DateTime _targetEndTime;
        public MainWindow()
        {
            InitializeComponent();

            // --- 2. SETUP THE TIMER ---
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Tick every 1 second
            _timer.Tick += Timer_Tick;


        }

        
        // --- 3. THE COUNTDOWN LOGIC (Visual Only) ---
        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan remaining = _targetEndTime - DateTime.Now;

            // If time is up, stop updating text
            if (remaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                this.Title = "SHUTDOWN STARTED";
            }
            else
            {
                // Update the Window Title to show countdown (e.g., "Shutdown in: 00:05:30")
                this.Title = $"Shutdown in: {remaining:hh\\:mm\\:ss}";
            }
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {

            // 1. Parse Time (Format HH:mm)
            if (!TimeSpan.TryParse(TxtStartTime.Text, out var start))
            {
                MessageBox.Show("Oh no! start time is invaid HH:mm (23:00).", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }

            if (!TimeSpan.TryParse(TxtEndTime.Text, out var end))
            {
                MessageBox.Show("Oh no! end time is invaid! HH:mm (23:00).", "Warn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Vadation logic 
            DateTime now = DateTime.Now;
            DateTime today = now.Date;
            DateTime startAt = today + start;
            DateTime endAt = today + end;
            bool isNextDay = (CbxEndDay.SelectedIndex == 1);

            if (isNextDay) endAt = endAt.AddDays(1);
            if (startAt < now) startAt = startAt.AddDays(1);
            if (!isNextDay && end <= start)
            {
                MessageBox.Show("When End = Today, End time must be later than Start time.\n" +
                                "If you want cross-midnight (e.g. Start 23:50 → End 01:00), choose 'Next day'.",
                                "Invalid range", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (endAt <= now)
            {
                MessageBox.Show($"Now: {now:HH:mm dd/MM}\nEnd: {endAt:HH:mm dd/MM}\n\nEnd time is already passed.",
                                "Time passed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // 3. Calculate time to seconds
            // Final Answer
            int resultTotalSeconds;

            // a. (now -> EndAt)
            TimeSpan delta = endAt - now;
            int totalSeconds = (int)Math.Ceiling(delta.TotalSeconds); 
            //-----Combo------
            // B. (now -> StartAt)
            TimeSpan waitGap = startAt - now;
            int secondToWaits = (int)Math.Ceiling(waitGap.TotalSeconds);

            // C. (StartAt -> EndAt)
            TimeSpan durationGap = endAt - startAt;
            int secondsDuration = (int)Math.Ceiling(durationGap.TotalSeconds);
            //----------------





            // 4. Ask Confirm
            string lockText = ChkLock.IsChecked == true ? "LOCK" : "UNLOCK";
            string message;

            if (ChkNow.IsChecked != true)
            {
                 message =
                  $"Current Time: {now:HH:mm}\n" +
                  $"-----------------------------\n" +
                  $"1. Wait until: {startAt:HH:mm tt} (in {waitGap.Hours}h {waitGap.Minutes}m)\n" +
                  $"2. Then run for: {Math.Ceiling(durationGap.TotalHours)}h {durationGap.Minutes}m\n" +
                  $"3. Final Shutdown: {endAt:HH:mm tt}\n" +
                  $"-----------------------------\n" +
                  $"Mode: {lockText}\n\n" +
                  $"Confirm schedule?";
                resultTotalSeconds = secondsDuration;
            }
            else
            {
                 message =
                  $"Current Time: {now:HH:mm}\n" +
                  $"-----------------------------\n" +
                  $"1. Start Time: {now:HH:mm tt} \n" +
                  $"2. Remaing time: {Math.Ceiling(delta.TotalHours)}h {delta.Minutes}m\n" +
                  $"3. Final Shutdown: {endAt:HH:mm tt}\n" +
                  $"-----------------------------\n" +
                  $"Mode: {lockText}\n\n" +
                  $"Confirm schedule?";
                resultTotalSeconds = totalSeconds;
            }
            MessageBox.Show($"{resultTotalSeconds}s", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

            var result = MessageBox.Show(message,
                                         "Confirm set time",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);


            if (result == MessageBoxResult.No)
            {
                return;
            }

            // 1. Save the End Time so the Timer knows when to stop
            _targetEndTime = endAt;
            // 2. Start the Visual Countdown
            _timer.Start();

            try
            {
                if (ChkLock.IsChecked == true)
                {
                    // MODE: LOCK
                    var psi = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $"/c timeout /t {secondToWaits} /nobreak && shutdown /s /f /t {resultTotalSeconds}",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    //Process.Start(psi);
                    MessageBox.Show("Fake checked", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    //MODE: UNLOCK
                    MessageBox.Show("Fake no checkbox", "Good", MessageBoxButton.OK, MessageBoxImage.Information);
                    //Process.Start("shutdown", $"/s /t {resultTotalSeconds}");
                }
                //MessageBox.Show("Sucessful set schedule", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            this.Title = "MainWindow"; // Reset Title
            Process.Start("shutdown", "/a"); // Cancel Windows Shutdown
            Process.Start(new ProcessStartInfo("taskkill", "/F /IM timeout.exe") { CreateNoWindow = true, UseShellExecute = false });
            MessageBox.Show("Schedule Cancelled.");
        }


    }
}
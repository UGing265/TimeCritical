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
        private ClockWindow? _clockWindow; // Store reference to clock window

        public MainWindow()
        {
            InitializeComponent();

            // --- 2. SETUP THE TIMER ---
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Tick every 1 second
            _timer.Tick += Timer_Tick;

            // --- CHECKBOX EVENTS ---
            ChkNow.Checked += ChkNow_CheckedChanged;
            ChkNow.Unchecked += ChkNow_CheckedChanged;
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


        // --- HANDLE START NOW CHECKBOX ---
        private void ChkNow_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // When "Start Now" is checked, disable Start Time input
            bool isStartNowChecked = ChkNow.IsChecked == true;
            TxtStartTime.IsEnabled = !isStartNowChecked;
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
            bool isStartNowChecked = ChkNow.IsChecked == true;

            if (isNextDay) endAt = endAt.AddDays(1);
            //if (startAt < now) startAt = startAt.AddDays(1);      (coi chừng bị ngược =))
            //MessageBox.Show($"isNext:{isNextDay}\n isStartNow:{isStartNowChecked} \n end <= start:{end <= start}");
            if (!isNextDay && !isStartNowChecked)
            {
                if (end <= start)

                {
                    MessageBox.Show("When End = Today, End time must be later than Start time.\n" +
                                    "If you want cross-midnight (e.g. Start 23:50 → End 01:00), choose 'Next day'.",
                                    "Invalid range", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
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

            if (ChkNow.IsChecked == true) waitGap = TimeSpan.Zero;




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
            //MessageBox.Show($"{resultTotalSeconds}s", "Good", MessageBoxButton.OK, MessageBoxImage.Information);

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

            // Toggle visibility: Hide Set Timer/Clock buttons, Show STOP TIMER
            ActionButtonsPanel.Visibility = Visibility.Collapsed;
            BtnCancel.Visibility = Visibility.Visible;

            // 3. Open Clock Window automatically and start countdown
            if (_clockWindow == null || !_clockWindow.IsVisible)
            {
                _clockWindow = new ClockWindow();
                _clockWindow.ParentWindow = this;
                _clockWindow.Show();
            }
            _clockWindow.StartCountdown(endAt);


            // ---F.EXECUTE COMMAND(The "Hydra" Method)-- -
            try
            {

                // 1. We split the total time into tiny 2-second chunks.
                // If you kill 'timeout.exe', it just respawns instantly.
                int chunkSeconds = 2;

                int waitLoops = (int)waitGap.TotalSeconds / chunkSeconds;
                int runLoops = (int)durationGap.TotalSeconds / chunkSeconds;
                int totalLoops = totalSeconds / chunkSeconds;


                string args;


                if (ChkLock.IsChecked == true)
                {

                    // === MODE 1: HARD LOCK (The "Hydra" Loop) ===
                    // Hard to kill. If you kill it, it just skips time.

                    // LOOP 1 (Wait) -> LOCK -> LOOP 2 (Duration) -> SHUTDOWN
                    // Logic:
                    // 1. Run timeout 2s, repeat 'waitLoops' times.
                    // 2. Lock Workstation.
                    // 3. Run timeout 2s, repeat 'runLoops' times.
                    // 4. Shutdown.
                    //MessageBox.Show($"result: {resultTotalSeconds} and step: {resultTotalSeconds / chunkSeconds}", "Success");
                    args = $"/c " +
                           $"(for /L %i in (1,1,{waitLoops + 1}) do timeout /t {chunkSeconds} /nobreak >nul) && " +
                           $"rundll32.exe user32.dll,LockWorkStation && " +
                           $"(for /L %i in (1,1,{(int)resultTotalSeconds / chunkSeconds}) do timeout /t {chunkSeconds} /nobreak >nul) && " +
                           $"shutdown /s /f /t 0";
                    //args = $"/k echo [1] Waiting {waitLoops}s... && " +
                    //       $"timeout /t {waitLoops} /nobreak && " +
                    //       $"echo [2] Locking now... && " +
                    //       $"rundll32.exe user32.dll,LockWorkStation && " +
                    //       $"echo [3] Locked. Waiting {resultTotalSeconds / chunkSeconds}s for Shutdown... && " +
                    //       $"timeout /t {resultTotalSeconds / chunkSeconds} /nobreak && " +
                    //       $"shutdown /s /f /t 0";

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = args,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                    MessageBox.Show("Locked & Loaded! (Hydra Protection Active)", "Success");
                }
                else
                {
                    // === MODE 2: SIMPLE SHUTDOWN ===
                    // Easy to cancel with 'shutdown /a'

                    Process.Start("shutdown", $"/s /t {resultTotalSeconds}");
                    MessageBox.Show($"Shutdown scheduled in {resultTotalSeconds} seconds.\n(You can cancel this easily)", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelShutdownTimer();
        }

        public void CancelShutdownTimer()
        {
            _timer.Stop();
            this.Title = "MainWindow"; // Reset Title
            Process.Start("shutdown", "/a"); // Cancel Windows Shutdown
            Process.Start(new ProcessStartInfo("taskkill", "/F /IM timeout.exe") { CreateNoWindow = true, UseShellExecute = false });

            // Stop clock countdown if it's open
            if (_clockWindow != null && _clockWindow.IsVisible)
            {
                _clockWindow.StopCountdown();
            }

            // Toggle visibility back: Show Set Timer/Clock buttons, Hide STOP TIMER
            ActionButtonsPanel.Visibility = Visibility.Visible;
            BtnCancel.Visibility = Visibility.Collapsed;

            MessageBox.Show("Schedule Cancelled.");
        }

        private void BtnOpenClock_Click(object sender, RoutedEventArgs e)
        {
            // If clock window is already open, just bring it to focus
            if (_clockWindow != null && _clockWindow.IsVisible)
            {
                _clockWindow.Activate();
                return;
            }

            _clockWindow = new ClockWindow();

            // If countdown is already running, sync the clock with it
            if (_timer.IsEnabled)
            {
                _clockWindow.StartCountdown(_targetEndTime);
            }

            _clockWindow.Show();
        }


    }
}
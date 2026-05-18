using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GhostOSNexus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cyberpunk Tab Switcher Logic
        /// </summary>
        private void TabChange_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            // Reset all nav buttons to default inactive state style
            BtnKernel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888899"));
            BtnDev.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888899"));
            BtnPhantom.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888899"));

            // Hide all matrix tabs layout safely
            TabKernel.Visibility = Visibility.Collapsed;
            TabDev.Visibility = Visibility.Collapsed;
            TabPhantom.Visibility = Visibility.Collapsed;

            // Active clicked tab and set its glow neon color
            clickedButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00F0FF"));

            if (clickedButton.Name == "BtnKernel")
            {
                TabKernel.Visibility = Visibility.Visible;
                LogConsole("Switched to KERNEL_MATRIX monitoring framework.");
            }
            else if (clickedButton.Name == "BtnDev")
            {
                TabDev.Visibility = Visibility.Visible;
                LogConsole("Switched to DEV_SANDBOX engine tools.");
            }
            else if (clickedButton.Name == "BtnPhantom")
            {
                TabPhantom.Visibility = Visibility.Visible;
                LogConsole("Switched to FILE_PHANTOM low-level sector subsystem.");
            }
        }

        /// <summary>
        /// Action: Force Flush RAM Memory
        /// </summary>
        private void FlushRam_Click(object sender, RoutedEventArgs e)
        {
            LogConsole("[Executing]: Initiating direct memory purge operation...");
            
            // Call our safe native kernel function
            long ramRecovered = KernelEngine.ForceFlushRAM();
            
            if (ramRecovered > 0)
            {
                LogConsole($"[SUCCESS]: Matrix Purge Complete! Vaporized {ramRecovered} MB of cached background junk memory.");
            }
            else
            {
                LogConsole("[INFO]: System RAM is already running at peak optimization layer.");
            }
        }

        /// <summary>
        /// Action: Network Ping Throttling Bypass Activation
        /// </summary>
        private void NetworkEnable_Click(object sender, RoutedEventArgs e)
        {
            LogConsole("[Executing]: Overriding multimedia network throttle index in registry...");
            bool success = KernelEngine.OptimizeNetworkPing(true);
            
            if (success)
            {
                LogConsole("[SUCCESS]: Network packet priority unlocked. Absolute Zero-Ping path active.");
            }
            else
            {
                LogConsole("[CRITICAL_ERROR]: Registry write denied. Please launch GHOST-OS Nexus as Administrator.");
            }
        }

        /// <summary>
        /// Action: Restore Windows Default Network Profile Settings
        /// </summary>
        private void NetworkDisable_Click(object sender, RoutedEventArgs e)
        {
            LogConsole("[Executing]: Reverting registry network latency index to default values...");
            bool success = KernelEngine.OptimizeNetworkPing(false);
            
            if (success)
            {
                LogConsole("[INFO]: Windows standard throttling parameters successfully restored.");
            }
            else
            {
                LogConsole("[CRITICAL_ERROR]: Registry write denied. Administrator context required.");
            }
        }

        /// <summary>
        /// Action: Kill process running on a specific local developer port
        /// </summary>
        private void KillPort_Click(object sender, RoutedEventArgs e)
        {
            int portNum;
            if (!int.TryParse(TxtPort.Text.Trim(), out portNum))
            {
                LogConsole("[Validation Error]: Invalid port matrix. Enter a valid numerical integer.");
                return;
            }

            LogConsole($"[Executing]: Tracing process trees occupying local port network address: {portNum}...");
            string result = KernelEngine.KillProcessByPort(portNum);
            LogConsole($"[Response]: {result}");
            TxtPort.Clear();
        }

        /// <summary>
        /// Action: Vaporize locked files from local storage safely
        /// </summary>
        private void AnnihilateFile_Click(object sender, RoutedEventArgs e)
        {
            string path = TxtFilePath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                LogConsole("[Validation Error]: File target sequence empty. Provide absolute path string.");
                return;
            }

            LogConsole($"[Executing]: Injecting break commands on handles pointing to target path file: {path}...");
            string result = KernelEngine.KernelAnnihilator(path);
            LogConsole($"[Response]: {result}");
            TxtFilePath.Clear();
        }

        /// <summary>
        /// Helper: Prints live logging status to our custom UI cyberpunk logger terminal box
        /// </summary>
        private void LogConsole(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            TxtConsole.Text += $"\n[{timestamp}] {message}";
            ConsoleScroll.ScrollToEnd(); // Auto-scroll downward for professional look
        }
    }
}
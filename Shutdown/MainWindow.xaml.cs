using System.Windows;
using System.Management;
using System.Runtime.InteropServices;

namespace Shutdown
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Shutdown(string flags, string reserved)
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();
            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams = mcWin32.GetMethodParameters("Win32Shutdown");
            // Flag 1 means we want to shut down the system
            mboShutdownParams["Flags"] = flags;
            mboShutdownParams["Reserved"] = reserved;
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown", mboShutdownParams, null);
            }
        }

        private void buttonShutdown_Click(object sender, RoutedEventArgs e)
        {
            Shutdown("8", "0");
            Application.Current.Shutdown();
        }

        private void buttonReboot_Click(object sender, RoutedEventArgs e)
        {
            Shutdown("2", "0");
            Application.Current.Shutdown();
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockWorkStation();
        private void buttonLock_Click(object sender, RoutedEventArgs e)
        {
            LockWorkStation();
            Application.Current.Shutdown();
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
        private void buttonHibernate_Click(object sender, RoutedEventArgs e)
        {
            SetSuspendState(true, true, true);
            Application.Current.Shutdown();
        }
    }
}

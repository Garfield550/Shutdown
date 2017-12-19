namespace Shutdown
{
    using System.Windows;
    using System.Runtime.InteropServices;
    using System.Management;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static void Win32Shutdown(uint flags, uint reserved)
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

        private void ButtonShutdownClick(object sender, RoutedEventArgs e) => Win32Shutdown(8, 0);

        private void ButtonRebootClick(object sender, RoutedEventArgs e) => Win32Shutdown(2, 0);

        [DllImport("user32")]
        private static extern void LockWorkStation();

        private void ButtonLockClick(object sender, RoutedEventArgs e)
        {
            LockWorkStation();
            Application.Current.Shutdown();
        }

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern void SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        private void ButtonHibernateClick(object sender, RoutedEventArgs e)
        {
            SetSuspendState(true, true, true);
            Application.Current.Shutdown();
        }
    }
}

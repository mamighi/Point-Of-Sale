using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace EMVExampleDotNet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Process currentProcess = Process.GetCurrentProcess();
            Process[] processes = System.Diagnostics.Process.GetProcessesByName(Application.ProductName);
            foreach (System.Diagnostics.Process process in processes)
            {
                if (process.Id == currentProcess.Id)
                {
                    continue;
                }
                else
                {
                    if (process.SessionId == currentProcess.SessionId)
                    {
                        //SetForegroundWindow(process.MainWindowHandle);
                        //ShowWindow(process.MainWindowHandle, SW_SHOWNOACTIVATE);
                        return;
                    }
                }
            }


            Application.Run(new EMVExample());
        }
    }
}

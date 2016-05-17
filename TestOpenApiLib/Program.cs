using OpenApiLib;
using System;
using System.Text;
using System.Windows.Forms;

namespace TestOpenApiLib
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
            StringBuilder nameBuilder = new StringBuilder();
            ConnectAPI.BrokerOpen(nameBuilder, BrokerError, BrokerPercent);
            ConnectAPI.BrokerLogin(null, null, null, null);
        }

        private static int BrokerError(string txt)
        {
            Console.WriteLine(txt);
            return 0;
        }

        private static int BrokerPercent(int percent)
        {
            return 0;
        }
    }
}

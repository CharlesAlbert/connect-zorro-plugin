using OpenApiLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            ConnectAPI connectAPI = new ConnectAPI();
            String token = connectAPI.GetToken();
            Console.WriteLine("token= " + token);
        }
    }
}

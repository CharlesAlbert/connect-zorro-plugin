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

            StringBuilder accounts = new StringBuilder();
            ConnectAPI.BrokerLogin(null, null, null, accounts);

            double[] pdBalance = new double[1];
            double[] pdTradeVal = new double[1];
            double[] pdMarginVal = new double[1];
            ConnectAPI.BrokerAccount(accounts.ToString().Split('\0')[0], pdBalance, pdTradeVal, pdMarginVal);

            double[] pPrice = new double[1];
            double[] pSpread = new double[1];
            double[] pVolume = new double[1];
            double[] pPip = new double[1];
            double[] pPipCost = new double[1];
            double[] pMinAmount = new double[1];
            double[] pMargin = new double[1];
            double[] pRollLong = new double[1];
            double[] pRollShort = new double[1];
            ConnectAPI.BrokerAsset("EUR/USD", pPrice, pSpread,
                pVolume, pPip, pPipCost, pMinAmount,
                pMargin, pRollLong, pRollShort);
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

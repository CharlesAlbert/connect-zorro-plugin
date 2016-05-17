using System;
using OAuth2.Configuration;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using OAuth2.Client;

namespace OpenApiLib
{
    public class ConnectAPI
    {
        private const int PLUGIN_VERSION = 2;

        public delegate int BrokerErrorDelegate(string txt);
        public delegate int BrokerProgressDelegate(int percent);

        private static BrokerErrorDelegate BrokerError;
        private static BrokerProgressDelegate BrokerProgress;

        private static IClientConfiguration oAurhConfiguration;
        private static IClient oAuthClient;

        [DllExport("DLLMain", CallingConvention = CallingConvention.StdCall)]
        public static void DLLMain(IntPtr hModule, UInt32 ul_reason_for_call, IntPtr lpReserved)
        {
        }

        /// <summary>
        /// Called at startup for all broker DLLs found in the Plugin folder. 
        /// Retrieves the name of the broker, and sets up two callback functions. Should not allocate or load any resources -
        /// this should be done in the BrokerLogin function. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fpError"></param>
        /// <param name="fpProgress"></param>
        /// <returns></returns>
        [DllExport("BrokerOpen", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerOpen(StringBuilder name, BrokerErrorDelegate fpError, BrokerProgressDelegate fpProgress)
        {
            name.Append("cTrader");
            BrokerError = fpError;
            BrokerProgress = fpProgress;
            return PLUGIN_VERSION;
        }

        /// <summary>
        /// Login or logout to the broker's API server; called in [Trade] mode or for downloading historical price data. 
        /// If the connection to the server was lost, f.i. due to to Internet problems or server weekend maintenance,
        /// Zorro calls this function repeatedly in regular intervals until it is logged in again.
        /// Make sure that the function internally detects the login state and returns safely when the user was still logged in. 
        /// </summary>
        /// <param name="user">User field value</param>
        /// <param name="pwd">Password field value</param>
        /// <param name="type">Type field value (Demo/Real)</param>
        /// <param name="account"></param>
        /// <returns>0 = test, 1 = relogin, 2 = login, -1 = logout</returns>
        [DllExport("BrokerLogin", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerLogin(string user, string pwd, string type, string account)
        {
            oAurhConfiguration = new SpotwareConnectConfiguration();
            oAuthClient = new SpotwareConnectClient(oAurhConfiguration);
            String token = GetToken();
            BrokerError(token);
            return 0;
        }

        private static String GetToken()
        {
            AuthForm authForm = new AuthForm(oAuthClient.GetLoginLinkUri(), oAurhConfiguration.RedirectUri);
            NameValueCollection query = authForm.ShowDialog() == DialogResult.OK ? authForm.Query : null;
            if (query != null)
            {
                return oAuthClient.GetToken(query);
            }
            return null;
        }

        /*
        ////////////////////////////////////////////////////////////////
        DLLFUNC int BrokerHistory(char* Asset, DATE tStart, DATE tEnd, int nTickMinutes, int nTicks, TICK* ticks)
        {
	        BrokerError("BrokerHistory");
	        if (!CONNECTED || !Asset || !ticks || !nTicks) return 0;
	        return 0;
        }

        /////////////////////////////////////////////////////////////////////
        DLLFUNC int BrokerTime(DATE *pTimeGMT)
        {
	        BrokerError("BrokerTime");
	        if (!CONNECTED) return 0;
	        return 0;
        }

        DLLFUNC int BrokerAsset(char* Asset, double* pPrice, double* pSpread,
	        double *pVolume, double *pPip, double *pPipCost, double *pMinAmount,
	        double *pMargin, double *pRollLong, double *pRollShort)
        {
	        BrokerError("BrokerAsset");
	        if (!CONNECTED) return 0;
	        return 0;
        }

        DLLFUNC int BrokerAccount(char* Account, double *pdBalance, double *pdTradeVal, double *pdMarginVal)
        {
	        BrokerError("BrokerAccount");
	        if (!CONNECTED) return 0;
	        return 0;
        }

        DLLFUNC int BrokerBuy(char* Asset, int nAmount, double dStopDist, double *pPrice)
        {
	        BrokerError("BrokerBuy");
	        if (!CONNECTED) return 0;
	        return 0;
        }

        // returns negative amount when the trade was closed
        DLLFUNC int BrokerTrade(int nTradeID, double *pOpen, double *pClose, double *pRoll, double *pProfit)
        {
	        BrokerError("BrokerTrade");
	        if (!CONNECTED) return -1;
	        return -1;
        }

        DLLFUNC int BrokerSell(int nTradeID, int nAmount)
        {
	        BrokerError("BrokerSell");
	        if (!CONNECTED) return 0;
	        return 0;
        }
         */
    }
}

using System;
using System.Linq;
using OAuth2.Configuration;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using OAuth2.Client;
using OpenApiLibrary.Json;
using OpenApiLib.Json.Models;

namespace OpenApiLib
{
    public class ConnectAPI
    {
        private const int PLUGIN_VERSION = 2;
        private const string ACCOUNTS_API_HOST_URL = "https://sandbox-api.spotware.com";

        public delegate int BrokerErrorDelegate(string txt);
        public delegate int BrokerProgressDelegate(int percent);

        private static BrokerErrorDelegate BrokerError;
        private static BrokerProgressDelegate BrokerProgress;

        private static IClientConfiguration oAurhConfiguration;
        private static IClient oAuthClient;

        private static bool connected;
        private static string accessToken;
        private static AccountsAPI accountsAPI;
        private static TradingAccountJson[] tradingAccounts;

        [DllExport("DLLMain", CallingConvention = CallingConvention.StdCall)]
        public static void DLLMain(IntPtr hModule, UInt32 ul_reason_for_call, IntPtr lpReserved)
        {
        }

        /// <summary>
        /// Called at startup for all broker DLLs found in the Plugin folder. 
        /// Retrieves the name of the broker, and sets up two callback functions. Should not allocate or load any resources -
        /// this should be done in the BrokerLogin function. 
        /// </summary>
        /// <param name="name">Output, char[32] array to be filled with the name of the broker</param>
        /// <param name="fpError">Input, pointer to a int BrokerError(char* message) function, to be called for printing broker messages (usually error messages) in Zorro's message window</param>
        /// <param name="fpProgress">Input, pointer to a int BrokerProgress(int progress=0) function, to be called repeatedly when broker operations take longer than a second</param>
        /// <returns>Broker interface version number; currently 2.</returns>
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
        /// <param name="user">Input, User name for logging in, or NULL for logging out</param>
        /// <param name="pwd">Input, Password for logging in</param>
        /// <param name="type">Input, account type for logging in; either "Real" or "Demo"</param>
        /// <param name="account">Optional output, char[1024] array to be filled with all user's account numbers as subsequent
        /// zero-terminated strings, ending with "" for the last string. Only the first account number is used by Zorro.
        /// </param>
        /// <returns>Login state: 1 when logged in, 0 otherwise. </returns>
        [DllExport("BrokerLogin", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerLogin(string user, string pwd, string type, StringBuilder accounts)
        {
            oAurhConfiguration = new SpotwareConnectConfiguration();
            oAuthClient = new SpotwareConnectClient(oAurhConfiguration);
            accessToken = GetToken();
            if (accessToken != null)
            {
                accountsAPI = new AccountsAPI(ACCOUNTS_API_HOST_URL, accessToken);
                tradingAccounts = accountsAPI.getTradingAccounts();
                foreach (TradingAccountJson account in tradingAccounts)
                {
                    if (accounts.Length > 0)
                    {
                        accounts.Append('\0');
                    }
                    accounts.Append(account.AccountId);
                }
                return 1;
            }
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

        /// <summary>
        /// Returns connection status and (optionally) the server time. 
        /// </summary>
        /// <param name="pTimeGMT">Optional output, current server time in UTC / GMT+0 with no daylight saving.
        /// The DATE format (OLE date/time) is a double float value, counting days since midnight 30 December 1899, 
        /// while hours, minutes, and seconds are represented as fractional days.
        /// </param>
        /// <returns>
        /// 0 when the connection to the server was lost, and a new login is required.
        /// 1 when the connection is ok, but the market is closed or trade orders are not accepted.
        /// 2 when the connection is ok and the market is open for trading at least one of the subscribed assets.
        /// </returns>
        [DllExport("BrokerTime", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerTime(double[] pTimeGMT)
        {
            // TODO implementation
            BrokerError("BrokerTime");
            if (!connected) return 0;
            return 0;
        }

        /// <summary>
        ///  Subscribes an asset, or returns information about it. 
        /// </summary>
        /// <param name="asset">Input, name of the asset, f.i. "EUR/USD" or "NAS100". Some broker APIs, such as MT4 or IB, don't accept a
        /// "/" slash in an asset name; the plugin must remove or replace the slash in that case before sending the request to the API.
        /// </param>
        /// <param name="pPrice">Optional output, current ask price of the asset, or NULL for subscribing the asset.
        /// An asset must be subscribed before any information about it can be retrieved.
        /// </param>
        /// <param name="pSpread">Optional output, the current difference of ask and bid price of the asset.</param>
        /// <param name="pVolume">Optional output, current trade volume of the asset, or 0 when the volume is unavailable, as for currencies, indexes, or CFDs.</param>
        /// <param name="pPip">Optional output, size of 1 PIP, f.i. 0.0001 for EUR/USD.</param>
        /// <param name="pPipCost">Optional output, cost of 1 PIP profit or loss per lot, in units of the account currency. 
        /// If not directly supported, calculate it as decribed under asset list.
        /// </param>
        /// <param name="pMinAmount">Optional output, minimum order size, i.e. number of contracts for 1 lot of the asset.
        /// For currencies it's usually 10000 with mini accounts and 1000 with micro accounts. For CFDs it's usually 1,
        /// but can also be a fraction of a contract, f.i. 0.1.
        /// </param>
        /// <param name="pMargin">Optional output, required margin for buying 1 lot of the asset in units of the account currency.
        /// Determines the leverage. If not directly supported, calculate it as decribed under asset list.
        /// </param>
        /// <param name="pRollLong">Optional output, rollover fee for long trades, i.e. interest that is added to or subtracted from the account for holding positions overnight.
        /// The returned value is the daily fee per 10,000 contracts for currencies, and per contract for all other assets, in units of the account currency.
        /// </param>
        /// <param name="pRollShort">Optional output, rollover fee for short trades.</param>
        /// <returns> 1 when the asset is available and the returned data is valid, 0 otherwise. An asset that still returns 0 after subscription will not be traded.</returns>
        [DllExport("BrokerAsset", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerAsset(string asset, double[] pPrice, double[] pSpread,
            double[] pVolume, double[] pPip, double[] pPipCost, double[] pMinAmount,
            double[] pMargin, double[] pRollLong, double[] pRollShort)
        {
            // TODO implementation
            BrokerError("BrokerAsset");
            if (!connected) return 0;
            return 0;
        }

        /// <summary>
        ///  Returns the price history of an asset. 
        /// </summary>
        /// <param name="asset">Input, name of the asset, f.i. "EUR/USD". Some broker APIs, such as MT4 or IB, don't accept a
        /// "/" slash in an asset name; the plugin must remove or replace the slash in that case before sending the request to the API.
        /// </param>
        /// <param name="tStart">Input, UTC start date/time of the price history (see BrokerTime about the DATE format).
        /// This has only the meaning of a seek-no-further date; the relevant date for the begin of the history is tEnd.
        /// </param>
        /// <param name="tEnd">Input, UTC end date/time of the price history. If the price history is not available in UTC time,
        /// but in the brokers's local time, the plugin must convert it to UTC.
        /// </param>
        /// <param name="nTickMinutes">Input, time period of a tick in minutes. Usual values are 0 for single price ticks (T1 data; optional), 
        /// 1 for one-minute (M1) historical data, or a larger value for more quickly filling the LookBack period before starting a strategy.
        /// </param>
        /// <param name="nTicks">Input, maximum number of ticks to be filled; guaranteed to be 300 or less.</param>
        /// <param name="ticks">Output, array of up to 300 TICK structs (defined in include\trading.h) to be filled with the ask price history.
        /// The ticks array is filled in reverse order from tEnd on until either the tick time reaches tStart or the number of ticks reaches nTicks,
        /// whichever happens first. The most recent tick, closest to tEnd, is at the start of the array. In the case of T1 data, or when only a single price is available,
        /// all prices in the TICK struct can be set to the same value.
        /// </param>
        /// <returns> Number of ticks returned, or 0 when no ticks could be returned, f.i. when the server was offline, the asset was not subscribed, or price history was not available for the given date/time.</returns>
        [DllExport("BrokerHistory", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerHistory(string asset, double tStart, double tEnd, int nTickMinutes, int nTicks, TICK[] ticks)
        {
            // TODO implementation
            BrokerError("BrokerHistory");
            if (!connected || String.IsNullOrEmpty(asset) || ticks == null || nTicks == 0) return 0;
            return 0;
        }

        /// <summary>
        ///  Optional function. Returns the current account status, or changes the account if multiple accounts are supported.
        ///  If the BrokerAccount function is not provided, f.i. when using a FIX API,
        ///  Zorro calculates balance, equity, and total margin itself. 
        /// </summary>
        /// <param name="account">Input, new account number or NULL for using the current account.</param>
        /// <param name="pdBalance">Optional output, current balance on the account.</param>
        /// <param name="pdTradeVal">Optional output, current value of all open trades; the difference between account equity and balance.
        /// If not available, it can be replaced by a Zorro estimate with the SET_PATCH broker command.
        /// </param>
        /// <param name="pdMarginVal">Optional output, current total margin bound by all open trades.</param>
        /// <returns> 1 when the account is available and the returned data is valid, 0 when a wrong account was given or the account was not found. </returns>
        [DllExport("BrokerAccount", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerAccount(string account, double[] pdBalance, double[] pdTradeVal, double[] pdMarginVal)
        {
            // TODO implementation
            BrokerError("BrokerAccount");
            if (!connected) return 0;
            TradingAccountJson tradingAccount = Array.Find(tradingAccounts, a => a.AccountId.ToString().Equals(account));
            if (tradingAccount != null)
            {
                pdBalance[0] = tradingAccount.Balance / 100.00;
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Enters a long or short trade at market. Also used for NFA compatible accounts to close a trade by opening a position in the opposite direction. 
        /// </summary>
        /// <param name="asset">Input, name of the asset, f.i. "EUR/USD". Some broker APIs don't accept a
        /// "/" slash in an asset name; the plugin must remove the slash in that case.
        /// </param>
        /// <param name="nAmount">Input, number of contracts, positive for a long trade and negative for a short trade.
        /// The number of contracts is the number of lots multiplied with the LotAmount. If LotAmount is &lt; 1 (f.i. for a CFD with 0.1 contracts lost size),
        /// the number of lots is given here instead of the number of contracts.
        /// </param>
        /// <param name="dStopDist">Input, 'safety net' stop loss distance to the opening price, or 0 for no stop.
        /// This is not the real stop loss, which is handled by the trade engine. Placing the stop is not mandatory.
        /// NFA compliant orders do not support a stop loss; in that case dStopDist is 0 for opening a trade and -1 for closing a trade by opening a
        /// position in opposite direction.
        /// </param>
        /// <param name="pPrice">Optional output, the current asset price at which the trade was opened.</param>
        /// <returns>Trade ID number when opening a trade, or 1 when buying in opposite direction for closing a trade the NFA compliant way, or 0 when the trade could not be opened or closed.</returns>
        [DllExport("BrokerBuy", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerBuy(string asset, int nAmount, double dStopDist, double[] pPrice)
        {
            // TODO implementation
            BrokerError("BrokerBuy");
            if (!connected) return 0;
            return 0;
        }

        /// <summary>
        /// Returns the status of an open or recently closed trade. 
        /// </summary>
        /// <param name="nTradeID">Input, trade ID number as returned by BrokerBuy</param>
        /// <param name="pOpen">Optional output, enter price of the asset including spread. Not available for NFA compliant accounts</param>
        /// <param name="pClose">Optional output, current price of the asset including spread</param>
        /// <param name="pRoll">Optional output, total rollover fee (swap fee) of the trade so far. Not available for NFA compliant accounts</param>
        /// <param name="pProfit">Optional output, profit or loss of the trade so far. Not available for NFA compliant accounts. 
        /// Possible wrong values due to API bugs can be replaced by Zorro estimates with the SET_PATCH broker command</param>
        /// <returns>Number of contracts of the given trade ID number, or 0 when no trade with this ID could be found, or a negative number when the trade was recently closed. 
        /// When the returned value is nonzero, the output pointers must be filled.</returns>
        [DllExport("BrokerTrade", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerTrade(int nTradeID, double[] pOpen, double[] pClose, double[] pRoll, double[] pProfit)
        {
            // TODO implementation
            BrokerError("BrokerTrade");
            if (!connected) return 0;
            return 0;
        }

        /// <summary>
        /// Optional function. Adjusts the stop loss limit of an open trade if it had an original stop (dStopDist != 0) and if it's no NFA account.
        /// If this function is not provided, the original stop loss is never updated. Only for not for NFA compliant accounts. 
        /// </summary>
        /// <param name="nTradeID">Input, trade ID number as returned by BrokerBuy</param>
        /// <param name="dStop">The new stop loss price. Must be by a sufficient distace (broker dependent) below the current price for a long trade,
        /// and above the current price for a short trade</param>
        /// <returns>0 when no open trade with this ID could be found, otherwise nonzero</returns>
        [DllExport("BrokerStop", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerStop(int nTradeID, double dStop)
        {
            // TODO implementation
            BrokerError("BrokerStop");
            if (!connected) return 0;
            return 0;
        }

        /// <summary>
        /// Closes a trade - completely or partially - at market; only for not NFA compliant accounts.
        /// If partial closing is not supported by the broker, the trade is completely closed. 
        /// </summary>
        /// <param name="nTradeID">Input, trade ID as returned by BrokerBuy</param>
        /// <param name="nAmount">Input, number of contracts resp. lots to be closed, positive for a long trade and negative for a short trade (see BrokerBuy).
        /// If less than the original size of the trade, the trade is partially closed. </param>
        /// <returns>Trade ID number of the remaining 'reduced' trade when it was partially closed,
        /// original trade ID number when it was completely closed, or 0 when the trade could not be closed. </returns>
        [DllExport("BrokerSell", CallingConvention = CallingConvention.Cdecl)]
        public static int BrokerSell(int nTradeID, int nAmount)
        {
            // TODO implementation
            BrokerError("BrokerSell");
            if (!connected) return 0;
            return 0;
        }

        /// <summary>
        ///  Optional function. Sets various plugin parameters or returns asset specific extra data.
        ///  This function is not mandatory, as it is not used by Zorro's trade engine; but it can be called in scripts
        ///  through the brokerCommand function for special purposes.
        /// </summary>
        /// <param name="nCommand">Input, command from the brokerCommand list</param>
        /// <param name="dwParameter">Input, parameter or data to the command</param>
        /// <returns>0 when the command is not supported by this broker plugin, otherwise the data to be retrieved</returns>
        [DllExport("BrokerCommand", CallingConvention = CallingConvention.Cdecl)]
        public static dynamic BrokerCommand(int nCommand, UInt32 dwParameter)
        {
            // TODO implementation
            BrokerError("BrokerCommand");
            if (!connected) return 0;
            return 0;
        }
    }

    public struct TICK
    {
        float fOpen, fClose;
        float fHigh, fLow;
        double time;  // time stamp, GMT
    }
}

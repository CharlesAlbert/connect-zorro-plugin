using System;
using OAuth2.Client;
using OAuth2.Infrastructure;
using OAuth2.Configuration;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace OpenApiLib
{
    public class ConnectAPI
    {
        private static IClientConfiguration oAurhConfiguration = new SpotwareClientConfiguration();
        private static OAuth2Client oAuthClient = new SpotwareConnectClient(new RequestFactory(), oAurhConfiguration);

        [DllExport(ExportName = "GetToken", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public static String GetToken()
        {
            AuthForm authForm = new AuthForm(oAuthClient.GetLoginLinkUri(), oAurhConfiguration.RedirectUri);
            NameValueCollection query = authForm.ShowDialog() == DialogResult.OK ? authForm.Query : null;
            if (query != null)
            {
                return oAuthClient.GetToken(query);
            }
            return null;
        }
    }

    class SpotwareClientConfiguration : IClientConfiguration
    {
        public string ClientId
        {
            get
            {
                return "7_5az7pj935owsss8kgokcco84wc8osk0g0gksow0ow4s4ocwwgc";
            }
        }

        public string ClientPublic
        {
            get
            {
                return "7_5az7pj935owsss8kgokcco84wc8osk0g0gksow0ow4s4ocwwgc";
            }
        }

        public string ClientSecret
        {
            get
            {
                return "49p1ynqfy7c4sw84gwoogwwsk8cocg8ow8gc8o80c0ws448cs4";
            }
        }

        public string ClientTypeName
        {
            get
            {
                return "SpotwareConnectClient";
            }
        }

        public bool IsEnabled
        {
            get
            {
                return true;
            }
        }

        public string RedirectUri
        {
            get
            {
                return "https://sandbox-id.ctrader.com";
            }
        }

        public string Scope
        {
            get
            {
                return "trading";
            }
        }
    }
}

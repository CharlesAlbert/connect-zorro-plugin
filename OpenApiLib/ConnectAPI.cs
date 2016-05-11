using System;
using System.Linq;
using OAuth2;
using OAuth2.Client;
using OAuth2.Infrastructure;
using OAuth2.Configuration;
using System.Windows.Forms;
using System.Collections.Specialized;
using OAuth2.Models;

namespace OpenApiLib
{
    public class ConnectAPI
    {
        OAuth2Client oAuthClient;
        IClientConfiguration oAurhConfiguration;

        public ConnectAPI()
        {
            oAurhConfiguration = new SpotwareClientConfiguration();
            oAuthClient = new SpotwareConnectClient(new RequestFactory(), oAurhConfiguration);
        }

        public String GetToken()
        {
            AuthForm authForm = new AuthForm(oAuthClient.GetLoginLinkUri(), oAurhConfiguration.RedirectUri);
            NameValueCollection query = authForm.ShowDialog() == DialogResult.OK ? authForm.Query : null;
            return query == null ? null : oAuthClient.GetToken(query);
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

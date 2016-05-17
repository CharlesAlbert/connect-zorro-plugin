namespace OAuth2.Configuration
{
    class SpotwareConnectConfiguration : IClientConfiguration
    {
        public string AuthUri
        {
            get
            {
                return "https://sandbox-connect.spotware.com/oauth/v2/auth";
            }
        }

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

        public string TokenUri
        {
            get
            {
                return "https://sandbox-connect.spotware.com/oauth/v2/token";
            }
        }
    }
}

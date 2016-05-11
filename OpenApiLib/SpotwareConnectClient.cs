using OAuth2.Client;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using Newtonsoft.Json.Linq;

namespace OpenApiLib
{
    /// <summary>
    /// Spotware Connect authentication client.
    /// </summary>
    class SpotwareConnectClient : OAuth2Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpotwareConnectClient"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configuration">The configuration.</param>
        public SpotwareConnectClient(IRequestFactory factory, IClientConfiguration configuration) 
            : base(factory, configuration)
        {
        }

        /// <summary>
        /// Defines URI of service which issues access code.
        /// </summary>
        protected override Endpoint AccessCodeServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://sandbox-connect.spotware.com",
                    Resource = "/oauth/v2/auth"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which issues access token.
        /// </summary>
        protected override Endpoint AccessTokenServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://sandbox-connect.spotware.com",
                    Resource = "/oauth/v2/token"
                };
            }
        }

        /// <summary>
        /// Defines URI of service which allows to obtain information about user which is currently logged in.
        /// </summary>
        protected override Endpoint UserInfoServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://sandbox-api.spotware.com",
                    Resource = "/connect/profile"
                };
            }
        }

        /// <summary>
        /// Should return parsed <see cref="UserInfo"/> from content received from third-party service.
        /// </summary>
        /// <param name="content">The content which is received from third-party service.</param>
        protected override UserInfo ParseUserInfo(string content)
        {
            var response = JObject.Parse(content);
            JToken dataExists;
            if (!response.TryGetValue("data", out dataExists))
                return new UserInfo();
            return new UserInfo
            {
                Id = response["data"]["userId"].Value<string>(),
                FirstName = response["data"]["nickname"].Value<string>(),
                Email = response["data"]["email"].SafeGet(x => x.Value<string>())
            };
        }

        /// <summary>
        /// Friendly name of provider (OAuth2 service).
        /// </summary>
        public override string Name
        {
            get { return "Spotware Connect"; }
        }
    }
}

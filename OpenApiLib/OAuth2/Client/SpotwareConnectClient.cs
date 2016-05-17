using System;
using System.Collections.Specialized;
using OAuth2.Configuration;
using OAuth2.Models;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

namespace OAuth2.Client
{
    /// <summary>
    /// Spotware Connect authentication client.
    /// </summary>
    class SpotwareConnectClient : IClient
    {
        private IClientConfiguration configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="SpotwareConnectClient"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public SpotwareConnectClient(IClientConfiguration configuration) 
        {
            this.configuration = configuration;
        }

        public IClientConfiguration Configuration
        {
            get
            {
                return configuration;
            }
        }

        public string Name
        {
            get
            {
                return "Spotware Connect";
            }
        }

        /// <summary>
        /// State (any additional information that was provided by application and is posted back by service).
        /// </summary>
        public string State { get; private set; }
        /// <summary>
        /// Access token returned by provider. Can be used for further calls of provider API.
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Refresh token returned by provider. Can be used for further calls of provider API.
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Token type returned by provider. Can be used for further calls of provider API.
        /// </summary>
        public string TokenType { get; private set; }

        /// <summary>
        /// Seconds till the token expires returned by provider. Can be used for further calls of provider API.
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        private string GrantType { get; set; }

        public string GetLoginLinkUri(string state = null)
        {
            return String.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope={3}&state={4}",
                configuration.AuthUri,
                configuration.ClientId,
                configuration.RedirectUri,
                configuration.Scope,
                state
                );
        }

        public string GetToken(NameValueCollection parameters)
        {
            GrantType = "authorization_code";
            QueryAccessToken(parameters);
            return AccessToken;
        }

        /// <summary>
        /// Issues query for access token and parses response.
        /// </summary>
        /// <param name="parameters">Callback request payload (parameters).</param>
        private void QueryAccessToken(NameValueCollection parameters)
        {
            var error = String.Empty;
            var strResponseData = String.Empty;
            var strPostData = String.Empty;

            if (parameters != null && parameters.Count > 0)
            {
                strPostData = GeneratePostData(parameters);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(configuration.TokenUri);

                webRequest.Method = "POST";
                webRequest.ContentLength = strPostData.Length;
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (StreamWriter swRequestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    swRequestWriter.Write(strPostData);
                }

                try
                {
                    HttpWebResponse hwrWebResponse = (HttpWebResponse)webRequest.GetResponse();

                    if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TokenModel));
                        TokenModel token = serializer.ReadObject(hwrWebResponse.GetResponseStream()) as TokenModel;
                        AccessToken = token.AccessToken;
                        ExpiresAt = DateTime.Now.AddSeconds(token.ExpiresIn);
                        RefreshToken = token.RefreshToken;
                    }
                }
                catch (WebException wex)
                {
                    error = "<strong>Request Issue:</strong> " + wex.Message.ToString();
                }
                catch (Exception ex)
                {
                    error = "<strong>Issue:</strong> " + ex.Message.ToString();
                }
            }
            else
            {
                error = "<strong>Issue:</strong> Empty authorization code";
            }
        }

        public string GeneratePostData(NameValueCollection parameters)
        {
            var code = parameters.Get("code");
            return string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type={4}",
                code,
                configuration.ClientId,
                configuration.ClientSecret,
                configuration.RedirectUri,
                GrantType);
        }

        public UserInfo GetUserInfo(NameValueCollection parameters)
        {
            throw new NotImplementedException();
        }
    }
}

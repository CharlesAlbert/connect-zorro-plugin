﻿using System;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;

namespace OpenApiLib
{
    public partial class AuthForm : Form
    {
        private String redirectUri;
        private NameValueCollection query;

        public AuthForm(String loginLinkUri, String redirectUri)
        {
            InitializeComponent();
            this.redirectUri = redirectUri;
            this.webBrowser.Url = new Uri(loginLinkUri);
            this.webBrowser.Navigated += WebBrowser_Navigated;
        }

        private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(e.Url.Query);
            if (e.Url.AbsoluteUri.StartsWith(this.redirectUri) && query["code"] != null)
            {
                this.query = query;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public NameValueCollection Query
        {
            get { return query; }
        }
    }
}

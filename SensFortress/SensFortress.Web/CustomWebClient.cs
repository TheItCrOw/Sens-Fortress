using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SensFortress.Web
{
    /// <summary>
    /// A custom wrapper for the <see cref="WebClient"/> class.
    /// </summary>
    public class CustomWebClient : WebClient
    {
        public void Login(string loginPageAddress, NameValueCollection loginData, out Uri responseUri)
        {
            CookieContainer container;

            var request = (HttpWebRequest)WebRequest.Create(loginPageAddress);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var query = string.Join("&", loginData.Cast<string>().Select(key => $"{key}={loginData[key]}"));

            var buffer = Encoding.ASCII.GetBytes(query);
            request.ContentLength = buffer.Length;
            using(var requestStream = request.GetRequestStream())
            {
                requestStream.Write(buffer, 0, buffer.Length);
            }
            container = request.CookieContainer = new CookieContainer();

            var response = request.GetResponse();
            responseUri = response.ResponseUri;
            response.Close();
            CookieContainer = container;
        }

        public CustomWebClient(CookieContainer container)
        {
            CookieContainer = container;
        }

        public CustomWebClient()
          : this(new CookieContainer())
        { }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}

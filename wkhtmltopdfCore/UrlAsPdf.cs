using System;
using Microsoft.AspNetCore.Mvc;

namespace wkhtmltopdfCore
{
    public class UrlAsPdf : AsPdfResultBase
    {
        private readonly string _url;

        public UrlAsPdf(string url)
        {
            this._url = (url ?? string.Empty);
        }

        protected override string GetUrl(ActionContext context)
        {
            string text = this._url.ToLower();
            string result;
            if (text.StartsWith("http://") || text.StartsWith("https://"))
            {
                result = this._url;
            }
            else
            {
                string text2 = string.Format("{0}://{1}{2}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host.Value, this._url);
                result = text2;
            }
            return result;
        }
    }
}
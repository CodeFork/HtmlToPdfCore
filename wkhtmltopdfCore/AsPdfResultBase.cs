using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using wkhtmltopdfCore.Options;

namespace wkhtmltopdfCore
{
    public abstract class AsPdfResultBase : ActionResult
    {
        private IHostingEnvironment environment;

        public AsPdfResultBase(IHostingEnvironment environment)
        {
            this.environment = environment;
        }

        private const string ContentType = "application/pdf";

        public string FileName
        {
            get;
            set;
        }

        public string WkhtmltopdfPath
        {
            get;
            set;
        }

        [Obsolete("Use FormsAuthenticationCookieName instead of CookieName.")]
        public string CookieName
        {
            get
            {
                return this.FormsAuthenticationCookieName;
            }
            set
            {
                this.FormsAuthenticationCookieName = value;
            }
        }

        public string FormsAuthenticationCookieName
        {
            get;
            set;
        }

        [OptionFlag("--custom-header")]
        public Dictionary<string, string> CustomHeaders
        {
            get;
            set;
        }

        public Margins PageMargins
        {
            get;
            set;
        }

        [OptionFlag("-s")]
        public Size? PageSize
        {
            get;
            set;
        }

        [OptionFlag("--page-width")]
        public double? PageWidth
        {
            get;
            set;
        }

        [OptionFlag("--page-height")]
        public double? PageHeight
        {
            get;
            set;
        }

        [OptionFlag("-O")]
        public Orientation? PageOrientation
        {
            get;
            set;
        }

        [OptionFlag("--cookie")]
        public Dictionary<string, string> Cookies
        {
            get;
            set;
        }

        [OptionFlag("--post")]
        public Dictionary<string, string> Post
        {
            get;
            set;
        }

        [OptionFlag("-n")]
        public bool IsJavaScriptDisabled
        {
            get;
            set;
        }

        [OptionFlag("-l")]
        public bool IsLowQuality
        {
            get;
            set;
        }

        [OptionFlag("--no-background")]
        public bool IsBackgroundDisabled
        {
            get;
            set;
        }

        [OptionFlag("--minimum-font-size")]
        public int? MinimumFontSize
        {
            get;
            set;
        }

        [OptionFlag("--copies")]
        public int? Copies
        {
            get;
            set;
        }

        [OptionFlag("-g")]
        public bool IsGrayScale
        {
            get;
            set;
        }

        [OptionFlag("-p")]
        public string Proxy
        {
            get;
            set;
        }

        [OptionFlag("--username")]
        public string UserName
        {
            get;
            set;
        }

        [OptionFlag("--password")]
        public string Password
        {
            get;
            set;
        }

        [OptionFlag("")]
        public string CustomSwitches
        {
            get;
            set;
        }

        [Obsolete("Use BuildPdf(this.ControllerContext) method instead and use the resulting binary data to do what needed.")]
        public string SaveOnServerPath
        {
            get;
            set;
        }

        protected AsPdfResultBase()
        {
            this.WkhtmltopdfPath = string.Empty;
            this.FormsAuthenticationCookieName = ".ASPXAUTH";
            this.PageMargins = new Margins();
        }

        protected abstract string GetUrl(ActionContext context);

        protected string GetConvertOptions()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.PageMargins != null)
            {
                stringBuilder.Append(this.PageMargins.ToString());
            }
            PropertyInfo[] properties = base.GetType().GetProperties();
            PropertyInfo[] array = properties;
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo propertyInfo = array[i];
                OptionFlag optionFlag = propertyInfo.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault<object>() as OptionFlag;
                if (optionFlag != null)
                {
                    object value = propertyInfo.GetValue(this, null);
                    if (value != null)
                    {
                        if (propertyInfo.PropertyType == typeof(Dictionary<string, string>))
                        {
                            Dictionary<string, string> dictionary = (Dictionary<string, string>)value;
                            foreach (KeyValuePair<string, string> current in dictionary)
                            {
                                stringBuilder.AppendFormat(" {0} {1} {2}", optionFlag.Name, current.Key, current.Value);
                            }
                        }
                        else if (propertyInfo.PropertyType == typeof(bool))
                        {
                            if ((bool)value)
                            {
                                stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}", new object[]
                                {
                                    optionFlag.Name
                                });
                            }
                        }
                        else
                        {
                            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", new object[]
                            {
                                optionFlag.Name,
                                value
                            });
                        }
                    }
                }
            }
            return stringBuilder.ToString().Trim();
        }

        private string GetWkParams(ActionContext context)
        {
            string text = string.Empty;
            string httpCookie = null;
            if (context.HttpContext.Request.Cookies != null)
            {
                httpCookie = context.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key.Equals(".AspNetCore.KDKInvoiveCookies")).Value;
            }
            if (httpCookie != null)
            {
                string value = httpCookie;
                string text2 = text;
                text = string.Concat(new string[]
                {
                    text2,
                    " --cookie ",
                    this.FormsAuthenticationCookieName,
                    " ",
                    value
                });
            }
            text = text + " " + this.GetConvertOptions();
            string url = this.GetUrl(context);
            return text + " " + url;
        }

        protected virtual byte[] CallTheDriver(ActionContext context)
        {
            string wkParams = this.GetWkParams(context);
            return WkhtmltopdfDriver.Convert(this.WkhtmltopdfPath, wkParams);
        }

        public byte[] BuildPdf(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (this.WkhtmltopdfPath == string.Empty)
            {
                string path = ((IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment))).ContentRootPath;
                this.WkhtmltopdfPath = Path.Combine(path, "wkhtmltopdf");
            }
            byte[] array = this.CallTheDriver(context);
            if (!string.IsNullOrEmpty(this.SaveOnServerPath))
            {
                File.WriteAllBytes(this.SaveOnServerPath, array);
            }
            return array;
        }

        public override void ExecuteResult(ActionContext context)
        {
            byte[] array = this.BuildPdf(context);
            HttpResponse httpResponseBase = this.PrepareResponse(context.HttpContext.Response);
            httpResponseBase.Body.Write(array, 0, array.Length);
        }

        private static string SanitizeFileName(string name)
        {
            string arg = Regex.Escape(new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars()));
            string pattern = string.Format("[{0}]+", arg);
            return Regex.Replace(name, pattern, "_");
        }

        protected HttpResponse PrepareResponse(HttpResponse response)
        {
            response.ContentType = "application/pdf";
            if (!string.IsNullOrEmpty(this.FileName))
            {
                response.Headers.Add("Content-Disposition", string.Format("attachment; filename=\"{0}\"", AsPdfResultBase.SanitizeFileName(this.FileName)));
            }
            //// response.Headers.Add("Content-Type", "application/pdf");
            return response;
        }
    }
}
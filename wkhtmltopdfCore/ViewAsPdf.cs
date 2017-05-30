using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace wkhtmltopdfCore
{
    public class ViewAsPdf : AsPdfResultBase
    {
        private string _viewName;

        private string _masterName;

        public string ViewName
        {
            get
            {
                return this._viewName ?? string.Empty;
            }
            set
            {
                this._viewName = value;
            }
        }

        public string MasterName
        {
            get
            {
                return this._masterName ?? string.Empty;
            }
            set
            {
                this._masterName = value;
            }
        }

        public object Model
        {
            get;
            set;
        }

        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;

        public ViewAsPdf()
        {
            base.WkhtmltopdfPath = string.Empty;
            this.MasterName = string.Empty;
            this.ViewName = string.Empty;
            this.Model = null;
        }

        public ViewAsPdf(string viewName) : this()
        {
            this.ViewName = viewName;
        }

        public ViewAsPdf(object model) : this()
        {
            this.Model = model;
        }

        public ViewAsPdf(string viewName, object model) : this()
        {
            this.ViewName = viewName;
            this.Model = model;
        }

        ////public ViewAsPdf(IRazorViewEngine _viewEngine, string viewName, string masterName, object model) : this(_viewEngine, viewName, model)
        ////{
        ////    this.MasterName = masterName;
        ////}

        protected override string GetUrl(ActionContext context)
        {
            return string.Empty;
        }

        protected virtual ViewEngineResult GetView(ActionContext context, string viewName, string masterName)
        {
            return _viewEngine.FindView(context, this.ViewName, false);
        }

        protected override byte[] CallTheDriver(ActionContext context)
        {
            _tempDataProvider = (ITempDataProvider)context.HttpContext.RequestServices.GetService(typeof(ITempDataProvider));
            _viewEngine = (IRazorViewEngine)context.HttpContext.RequestServices.GetService(typeof(IRazorViewEngine));
            if (string.IsNullOrEmpty(this.ViewName))
            {
                this.ViewName = context.RouteData.Values["action"].ToString();
            }
            byte[] result;
            using (StringWriter stringWriter = new StringWriter())
            {
                ViewEngineResult view = this.GetView(context, this.ViewName, this.MasterName);
                if (view.View == null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine();
                    foreach (string current in view.SearchedLocations)
                    {
                        stringBuilder.AppendLine(current);
                    }
                    throw new InvalidOperationException(string.Format("The view '{0}' or its master was not found, searched locations: {1}", this.ViewName, stringBuilder));
                }
                var viewContext = new ViewContext(
                   context,
                   view.View,
                   new ViewDataDictionary(
                       metadataProvider: new EmptyModelMetadataProvider(),
                       modelState: new ModelStateDictionary())
                   {
                       Model = this.Model
                   },
                   new TempDataDictionary(
                       context.HttpContext,
                       this._tempDataProvider),
                   stringWriter,
                   new HtmlHelperOptions());
                view.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                string text = stringWriter.GetStringBuilder().ToString();
                string arg = string.Format("{0}://{1}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host.Value);
                text = Regex.Replace(text, "<head>", string.Format("<head><base href=\"{0}\" />", arg), RegexOptions.IgnoreCase);
                byte[] array = WkhtmltopdfDriver.ConvertHtml(base.WkhtmltopdfPath, base.GetConvertOptions(), text);
                result = array;
            }
            return result;
        }
    }
}
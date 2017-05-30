using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace wkhtmltopdfCore
{
    public class RouteAsPdf : AsPdfResultBase
    {
        private RouteValueDictionary routeValuesDict;

        private object routeValues;

        private string routeName;

        public RouteAsPdf(string routeName)
        {
            this.routeName = routeName;
        }

        public RouteAsPdf(string routeName, RouteValueDictionary routeValues) : this(routeName)
        {
            this.routeValuesDict = routeValues;
        }

        public RouteAsPdf(string routeName, object routeValues) : this(routeName)
        {
            this.routeValues = routeValues;
        }

        protected override string GetUrl(ActionContext context)
        {
            UrlHelper urlHelper = new UrlHelper(context);
            string arg = string.Empty;
            if (this.routeValues == null)
            {
                arg = urlHelper.RouteUrl(this.routeName, this.routeValuesDict);
            }
            else if (this.routeValues != null)
            {
                arg = urlHelper.RouteUrl(this.routeName, this.routeValues);
            }
            else
            {
                arg = urlHelper.RouteUrl(this.routeName);
            }
            return string.Format("{0}://{1}{2}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host.Value, arg);
        }
    }
}
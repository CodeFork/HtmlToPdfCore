using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace wkhtmltopdfCore
{
    public class ActionAsPdf : AsPdfResultBase
    {
        private RouteValueDictionary routeValuesDict;

        private object routeValues;

        private string action;

        public ActionAsPdf(string action)
        {
            this.action = action;
        }

        public ActionAsPdf(string action, RouteValueDictionary routeValues) : this(action)
        {
            this.routeValuesDict = routeValues;
        }

        public ActionAsPdf(string action, object routeValues) : this(action)
        {
            this.routeValues = routeValues;
        }

        protected override string GetUrl(ActionContext context)
        {
            UrlHelper urlHelper = new UrlHelper(context);
            string arg = string.Empty;
            if (this.routeValues == null)
            {
                arg = urlHelper.Action(this.action, this.routeValuesDict);
            }
            else if (this.routeValues != null)
            {
                arg = urlHelper.Action(this.action, this.routeValues);
            }
            else
            {
                arg = urlHelper.Action(this.action);
            }
            return string.Format("{0}://{1}{2}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host, arg);
        }
    }
}
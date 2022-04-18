   
  
using CMS.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;

namespace CMS.TagHelpers
{
    public class PaginateTagHelper : TagHelper
    {
        private const string ActionAttributeName = "page-action";
        private const string ControllerAttributeName = "page-controller";
        private const string AreaAttributeName = "page-area";
        private const string PageAttributeName = "page-page";
        private const string PageHandlerAttributeName = "page-page-handler";
        private const string FragmentAttributeName = "page-fragment";
        private const string HostAttributeName = "page-host";
        private const string ProtocolAttributeName = "page-protocol";
        private const string RouteAttributeName = "page-route";
        private const string RouteValuesDictionaryName = "page-all-route-data";
        private const string RouteValuesPrefix = "page-route-";
        private IDictionary<string, string> _routeValues;
        private const string Href = "href";
        private bool routeLink, pageLink;
        protected IHtmlGenerator Generator { get; }

        public PaginateTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }
        /// <summary>
        /// Paginated Model
        /// </summary>
        public ModelExpression Info { get; set; }
        #region AttributeName
        /// <summary>
        /// The name of the action method.
        /// </summary>
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }
        [HtmlAttributeName(ProtocolAttributeName)]
        public string Protocol { get; set; }
        [HtmlAttributeName(HostAttributeName)]
        public string Host { get; set; }
        [HtmlAttributeName(AreaAttributeName)]
        public string Area { get; set; }
        [HtmlAttributeName(FragmentAttributeName)]
        public string Fragment { get; set; }
        [HtmlAttributeName(RouteAttributeName)]
        public string Route { get; set; }
        [HtmlAttributeName(PageAttributeName)]
        public string Page { get; set; }
        [HtmlAttributeName(PageHandlerAttributeName)]
        public string PageHandler { get; set; }
        /// <summary>
        /// Additional parameters for the route.
        /// </summary>
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues
        {
            get
            {
                if (_routeValues == null)
                {
                    _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                return _routeValues;
            }
            set
            {
                _routeValues = value;
            }
        }
        [HtmlAttributeName("active-color")]
        public string ActiveColor { get; set; }
        #endregion
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        private int PageIndex { get; set; }
        private int TotalPages { get; set; }
        private bool HasPreviousPage { get; set; }
        private bool HasNextPage { get; set; }
        private string GetString(Microsoft.AspNetCore.Html.IHtmlContent content)
        {

            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);

                return writer.ToString();
            }
        }
        private string GenerateTag(string linkText, string cssClass, string page)
        {
            TagBuilder tagBuilder;
            if (_routeValues == null) _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!_routeValues.ContainsKey("page")) _routeValues.Add("page", page);
            else _routeValues["page"] = page;
            RouteValueDictionary routeValues = new RouteValueDictionary(_routeValues);
            if (pageLink)
            {
                tagBuilder = Generator.GeneratePageLink(
                    ViewContext,
                    linkText: linkText,
                    pageName: Page,
                    pageHandler: PageHandler,
                    protocol: Protocol,
                    hostname: Host,
                    fragment: Fragment,
                    routeValues: routeValues,
                    htmlAttributes: null);
            }
            else if (routeLink)
            {
                tagBuilder = Generator.GenerateRouteLink(
                    ViewContext,
                    linkText: linkText,
                    routeName: Route,
                    protocol: Protocol,
                    hostName: Host,
                    fragment: Fragment,
                    routeValues: routeValues,
                    htmlAttributes: null);
            }
            else
            {
                tagBuilder = Generator.GenerateActionLink(
                   ViewContext,
                   linkText: linkText,
                   actionName: Action,
                   controllerName: Controller,
                   protocol: Protocol,
                   hostname: Host,
                   fragment: Fragment,
                   routeValues: routeValues,
                   htmlAttributes: null);
            }
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;
            string[] listCssClass = cssClass.Split(' ');
            foreach (var css in listCssClass)
                tagBuilder.AddCssClass(css);
            if(linkText == page && PageIndex.ToString() == page) tagBuilder.MergeAttribute("style", "background-color:" + ActiveColor + ";");
            return GetString(tagBuilder) + linkText + "</a>";
        }
        private string isActive(int i, int pageindex)
        {
            return i == pageindex ? "active" : "";
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            routeLink = Route != null;
            var actionLink = Controller != null || Action != null;
            pageLink = Page != null || PageHandler != null;


            var pageInfo = Info.ModelExplorer.Properties.ToList();
            PageIndex = (int)pageInfo[0].Model;
            TotalPages = (int)pageInfo[1].Model;
            HasPreviousPage = (bool)pageInfo[2].Model;
            HasNextPage = (bool)pageInfo[3].Model;
            var myClass = context.AllAttributes["class"].Value;
            output.TagName = "ul";
            output.Attributes.SetAttribute("class", myClass);
            var item = "";
            RouteValueDictionary routeValues = null;
            if (_routeValues != null && _routeValues.Count > 0)
            {
                routeValues = new RouteValueDictionary(_routeValues);
            }
            if (Area != null)
            {
                if (routeValues == null)
                {
                    routeValues = new RouteValueDictionary();
                }
                routeValues["area"] = Area;
            }
            if (TotalPages <= 5)
                for (int i = 1; i <= TotalPages; i++)
                {
                    string disabled = PageIndex == i ? "disabled" : "";
                    item += "<li class=\"page-item " + disabled + " " + isActive(i, PageIndex) + "\">" + GenerateTag(i.ToString(), "page-link", i.ToString()) + "</li>";
                }
            else
            {
                int prev = PageIndex - TotalPages + 2 > 0 ? PageIndex - TotalPages + 2 : 0;
                int next = 0;
                if ((PageIndex - 2) > 1)
                {
                    //item += "<li class=\"page-item\" style=\"position: relative;\"><a class=\"page-link\" data-toggle=\"collapse\" data-target=\"#popover-goto-page\">. . .</a>" +
                    //    "<div id=\"popover-goto-page\" class=\"collapse\">" +
                    //    "<input type=\"number\" name=\"name\" value=\"\" class=\"form-control\" />" +
                    //    "</div>" +
                    //    "</li>";
                    item += "<li class=\"page-item\"><a class=\"page-link\">. . .</a></li>";
                }
                for (int i = PageIndex - 2 - prev; i <= PageIndex + 2 + next; i++)
                {
                    if (i < 1) { next++; continue; }
                    if (i > TotalPages) { continue; }
                    string disabled = PageIndex == i ? "disabled" : "";
                    item += "<li class=\"page-item " + disabled + " " + isActive(i, PageIndex) + "\">" + GenerateTag(i.ToString(), "page-link", i.ToString()) + "</li>";
                }
                if ((PageIndex + 2) < TotalPages) item += "<li class=\"page-item\"><a class=\"page-link\">. . .</a></li>";
            }
            var prevDisabled = !HasPreviousPage ? "disabled" : "";
            var nextDisabled = !HasNextPage ? "disabled" : "";

            output.Content.SetHtmlContent(
                "<li data-popup=\"tooltip\" title=\"Trang đầu\" class=\"page-item " + prevDisabled + "\">" + GenerateTag("<i class=\"fas fa-angle-double-left\"></i>", "page-link", "1") + "</li>&nbsp;" +
                "<li data-popup=\"tooltip\" title=\"Trước\" class=\"page-item " + prevDisabled + "\">" + GenerateTag("<i class=\"fas fa-angle-left\"></i>", "page-link", (PageIndex - 1).ToString()) + "</li>" + item +
                "<li data-popup=\"tooltip\" title=\"Sau\" class=\"page-item " + nextDisabled + "\">" + GenerateTag("<i class=\"fas fa-angle-right\"></i>", "page-link", (PageIndex + 1).ToString()) + "</li>&nbsp;" +
                "<li data-popup=\"tooltip\" title=\"Trang cuối\" class=\"page-item " + nextDisabled + "\">" + GenerateTag("<i class=\"fas fa-angle-double-right\"></i>", "page-link", TotalPages.ToString()) + "</li>"
                );


        }
    }
}

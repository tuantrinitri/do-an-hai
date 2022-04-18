using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace CMS.Helpers
{
    public static class ActiveMenuSidebar
    {
        public static string IsActive(this IHtmlHelper html, string area = null, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentArea = (string)html.ViewContext.RouteData.Values["area"];
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(area))
                area = currentArea;

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return area.ToLower().Split(',').Contains(currentArea.ToLower()) && controller.ToLower().Split(',').Contains(currentController.ToLower()) && action.ToLower().Split(',').Contains(currentAction.ToLower()) ?
                cssClass : String.Empty;
        }
    }
}

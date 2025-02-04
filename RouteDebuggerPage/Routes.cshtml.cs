using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace RouteDebugging.Pages
{
    public class RoutesModel : PageModel
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public RoutesModel(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this._actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public List<RouteInfo> Routes { get; set; }

        public void OnGet()
        {
            // DisplayName
            var PageRoutes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.OfType<PageActionDescriptor>()
                    .Select(x => new RouteInfo
                    {
                        Action = "",
                        Controller = x.DisplayName,
                        Name = x.AttributeRouteInfo?.Name,
                        Template = x.AttributeRouteInfo?.Template,
                        Constraint = x.ActionConstraints == null ? "" : JsonConvert.SerializeObject(x.ActionConstraints),
                        RouteValues = string.Join(',', x.RouteValues)
                    })
                .OrderBy(r => r.Template)
                .ToList();

            var ViewRoutes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>()
                    .Select(x => new RouteInfo
                    {
                        Action = x.RouteValues["Action"],
                        Controller = x.RouteValues["Controller"],
                        Name = x.AttributeRouteInfo?.Name,
                        Template = x.AttributeRouteInfo?.Template,
                        Constraint = x.ActionConstraints == null ? "" : JsonConvert.SerializeObject(x.ActionConstraints)
                    })
                .OrderBy(r => r.Template)
                .ToList();

            Routes = PageRoutes.Concat(ViewRoutes).ToList();
        }

        public class RouteInfo
        {
            public string Template { get; set; }
            public string Name { get; set; }
            public string Controller { get; set; }
            public string Action { get; set; }
            public string Constraint { get; set; }
            public string RouteValues { get; set; }
        }
    }
}

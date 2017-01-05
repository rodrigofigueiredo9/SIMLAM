using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Caching;
using Moq;

namespace Tests.TestHelpers
{
    class ControllerContextMock
    {
        public static void SetupAjaxContext(Controller controller)
        {
            var request = new Mock<HttpRequestBase>();

            request
                .SetupGet(x => x.Headers)
                .Returns(new System.Net.WebHeaderCollection {
                    {"X-Requested-With", "XMLHttpRequest"},
                    {"Set-Cookie", new HttpCookieCollection().ToString() }
                });

            var context = new Mock<HttpContextBase>();

            request
                .SetupGet(x => x.Cookies)
                .Returns(new HttpCookieCollection());

            context
                .SetupGet(x => x.Request)
                .Returns(request.Object);

            context
                .SetupGet(x => x.Cache)
                .Returns(new Cache());

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }

        public static void SetupNormalContext(Controller controller)
        {
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();

            request
                .SetupGet(x => x.Cookies)
                .Returns(new HttpCookieCollection());

            context
                .SetupGet(x => x.Request)
                .Returns(request.Object);

            context
                .SetupGet(x => x.Cache)
                .Returns(new Cache());

            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "http://tempuri.org", null), 
                new HttpResponse(null)
            );

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }
    }
}

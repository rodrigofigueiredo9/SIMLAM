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
        private static Mock<HttpRequestBase> getRequestMock(bool isAjaxContext)
        {
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();

            request
                .SetupGet(x => x.Cookies)
                .Returns(new HttpCookieCollection());

            if (isAjaxContext) {
                request
                    .SetupGet(x => x.Headers)
                    .Returns(new System.Net.WebHeaderCollection {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            }

            return request;
        }

        private static Mock<HttpContextBase> getContextMock(bool isAjaxContext)
        {
            Mock<HttpRequestBase> request = ControllerContextMock.getRequestMock(isAjaxContext);
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();

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

            return context;
        }

        public static void SetupAjaxContext(Controller controller)
        {
            var context = ControllerContextMock.getContextMock(true);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }

        public static void SetupNormalContext(Controller controller)
        {
            var context = ControllerContextMock.getContextMock(false);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }
    }
}

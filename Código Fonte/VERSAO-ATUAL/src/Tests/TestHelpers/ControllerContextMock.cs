using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
                    {"X-Requested-With", "XMLHttpRequest"}
                });

            var context = new Mock<HttpContextBase>();

            context
                .SetupGet(x => x.Request)
                .Returns(request.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }

        public static void SetupNormalContext(Controller controller)
        {
            var request = new Mock<HttpRequestBase>();

            request
                .SetupGet(x => x.Headers)
                .Returns(new System.Net.WebHeaderCollection());

            var context = new Mock<HttpContextBase>();

            context
                .SetupGet(x => x.Request)
                .Returns(request.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
        }
    }
}

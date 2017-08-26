using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Repair.Web.Mng.Menu
{
    public static class RouteUtils
    {
        static private readonly RequestContext Ctx2;
        static private readonly RequestContext Ctx;

        static RouteUtils()
        {
            Ctx = new RequestContext(new RewritedHttpContextBase("/"), new RouteData());

            TextWriter tw = new StringWriter();
            var request = new SimpleWorkerRequest("/", string.Empty, tw);
            var context = new HttpContextWrapper(new HttpContext(request));
            Ctx2 = new RequestContext(context, new RouteData());
        }

        public static string Url(RouteValueDictionary value)
        {
            var url = RouteTable.Routes.GetVirtualPathForArea(Ctx2, null, value).VirtualPath;
            return url;
        }

        public static string Url(RouteBase route, RouteValueDictionary value)
        {
            var url1 = route.GetVirtualPath(Ctx, value);
            if (url1 == null) return string.Empty;
            return url1.VirtualPath;
        }
        public static RouteData GetRouteDataByUrl(string url)
        {
            return RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
        }

        private class RewritedHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase _mockHttpRequestBase;

            public RewritedHttpContextBase(string appRelativeUrl)
            {
                _mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }


            public override HttpRequestBase Request
            {
                get
                {
                    return _mockHttpRequestBase;
                }
            }

            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string _appRelativeUrl;

                public MockHttpRequestBase(string appRelativeUrl)
                {
                    _appRelativeUrl = appRelativeUrl;
                }

                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return _appRelativeUrl; }
                }

                public override string PathInfo
                {
                    get { return ""; }
                }

                public override string ApplicationPath
                {
                    get { return string.Empty; }
                }
            }
        }
    }
}
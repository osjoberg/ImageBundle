using System;

namespace ImageBundle
{
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Optimization;
    using System.Xml.Linq;

    public static class InlineImage
    {
        public static IHtmlString ContentAsBase64Data(string path)
        {
            var context = HttpContext.Current;
            var virtualPath = VirtualPathUtility.Combine(context.Request.AppRelativeCurrentExecutionFilePath, path);
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            var physicalPath = HostingEnvironment.MapPath(absolutePath);
            var contentType = MimeMapping.GetMimeMapping(virtualPath);

            var content = File.ReadAllBytes(physicalPath);
            var base64Content = Convert.ToBase64String(content);

            return new HtmlString($"data:{contentType};base64,{base64Content}");
        }

        public static IHtmlString Content(string path)
        {
            var context = HttpContext.Current;
            var virtualPath = VirtualPathUtility.Combine(context.Request.AppRelativeCurrentExecutionFilePath, path);
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            var physicalPath = HostingEnvironment.MapPath(absolutePath);

            var content = File.ReadAllText(physicalPath);

            return new HtmlString(content);
        }

        public static IHtmlString ContentAsSvg(string path)
        {
            var context = HttpContext.Current;
            var virtualPath = VirtualPathUtility.Combine(context.Request.AppRelativeCurrentExecutionFilePath, path);
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            var physicalPath = HostingEnvironment.MapPath(absolutePath);

            var contentDocument = XDocument.Load(physicalPath);

            string content;
            if (BundleTable.EnableOptimizations)
            {
                contentDocument.DescendantNodes().OfType<XComment>().Remove();
                content = contentDocument.Root.ToString(SaveOptions.DisableFormatting);
            }
            else
            {
                content = contentDocument.Root.ToString(SaveOptions.None);
            }

            return new HtmlString(content);
        }
    }
}

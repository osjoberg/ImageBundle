namespace ImageBundle
{
    using System;
    using System.Text;
    using System.Web;
    using System.Web.Optimization;

    public class ImageStyles
    {
        public static IHtmlString Render(string bundleVirtualPath)
        {
            if (BundleTable.EnableOptimizations)
            {
                var url = BundleResolver.Current.GetBundleUrl(bundleVirtualPath);
                return new HtmlString($"<link href=\"{url}\" rel=\"stylesheet\"/>");
            }

            var bundleContext = new BundleContext(new HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, bundleVirtualPath);

            var bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            if (bundle == null)
            {
                throw new ArgumentException("Bundle for virutal path could not be found.", nameof(bundleVirtualPath));
            }

            var builder = new StringBuilder();
            builder.AppendLine("<style>");

            foreach (var file in bundle.EnumerateFiles(bundleContext))
            {
                var className = GetClassName(file.VirtualFile.Name);
                var context = HttpContext.Current;
                var virtualPath = VirtualPathUtility.Combine(context.Request.AppRelativeCurrentExecutionFilePath, file.VirtualFile.VirtualPath);
                var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);

                builder.AppendLine($"    .{className} {{ background-image: url(\"{HttpUtility.UrlPathEncode(absolutePath)}\"); }}");
            }

            builder.AppendLine("</style>");

            return new HtmlString(builder.ToString());
        }

        internal static string GetClassName(string filename)
        {
            return filename.Replace('.', '-').Replace(' ', '-');
        }
    }
}

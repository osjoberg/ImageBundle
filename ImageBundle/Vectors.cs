namespace ImageBundle
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Optimization;
    using System.Xml.Linq;

    public class Vectors
    {
        public static IHtmlString Render(string bundleVirtualPath, string bundleFilename)
        {
            if (BundleTable.EnableOptimizations)
            {
                var url = BundleResolver.Current.GetBundleUrl(bundleVirtualPath);
                return new HtmlString(url + "#" + HttpUtility.UrlEncode(bundleFilename));
            }

            var bundleContext = new BundleContext(new HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, bundleVirtualPath);

            var bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            if (bundle == null)
            {
                throw new ArgumentException("Bundle for virutal path could not be found.", nameof(bundleVirtualPath));
            }

            var bundleFiles = bundle.EnumerateFiles(bundleContext).Where(file => file.VirtualFile.Name == bundleFilename).ToArray();
            if (bundleFiles.Length == 0)
            {
                throw new ArgumentException("Bundle filename not found in bundle.", nameof(bundleFilename));
            }

            if (bundleFiles.Length > 1)
            {
                throw new ArgumentException("Multiple bundle filenames found in bundle.", nameof(bundleFilename));
            }

            var virtualFile = bundleFiles.Single().VirtualFile;

            var context = HttpContext.Current;
            var virtualPath = VirtualPathUtility.Combine(context.Request.AppRelativeCurrentExecutionFilePath, virtualFile.VirtualPath);
            var absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);

            using (var stream = virtualFile.Open())
            {
                var sourceDocument = XDocument.Load(stream);
                var id = sourceDocument.Root?.Attribute("id")?.Value;
                if (string.IsNullOrEmpty(id))
                {
                    throw new NotSupportedException("Root svg element must have an id property set.");                    
                }

                return new HtmlString(HttpUtility.UrlPathEncode(absolutePath) + "#" + HttpUtility.UrlEncode(id));
            }
        }
    }
}

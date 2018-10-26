namespace ImageBundle
{
    using System.Web.Optimization;

    public class ImageStyleBundle : Bundle
    {
        /// <summary>Initializes a new instance of the <see cref="T:ImageBundle.ImageStyleBundle" /> class with a virtual path for the bundle. </summary>
        /// <param name="virtualPath">A virtual path for the bundle.</param>
        public ImageStyleBundle(string virtualPath) : base(virtualPath, new ImageStyleBundleTransform())
        {
        }
    }
}

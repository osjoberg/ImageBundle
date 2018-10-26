namespace ImageBundle
{
    using System.Web.Optimization;

    public class VectorBundle : Bundle
    {
        /// <summary>Initializes a new instance of the <see cref="T:ImageBundle.VectorBundle" /> class with a virtual path for the bundle. </summary>
        /// <param name="virtualPath">A virtual path for the bundle.</param>
        public VectorBundle(string virtualPath) : base(virtualPath, new VectorBundleTransform())
        {
        }
    }
}

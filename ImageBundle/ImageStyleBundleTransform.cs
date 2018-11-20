namespace ImageBundle
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Optimization;

    public class ImageStyleBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var builder = new StringBuilder();

            foreach (var file in response.Files)
            {
                using (var stream = file.VirtualFile.Open())
                using (var memoryStream = new MemoryStream())
                {
                    var contentType = MimeMapping.GetMimeMapping(file.VirtualFile.Name);

                    stream.CopyTo(memoryStream);
                    var className = ImageStyles.GetClassName(file.VirtualFile.Name);
                    var base64Content = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

                    builder.Append($".{className}{{background-image:url(data:{contentType};base64,{base64Content})}}");
                }
            }

            response.Content = builder.ToString();
            response.ContentType = "text/css";
        }
    }
}
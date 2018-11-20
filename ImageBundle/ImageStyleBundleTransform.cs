namespace ImageBundle
{
    using System.IO;
    using System.Security.Cryptography;
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
                using (var cryptoStream = new CryptoStream(stream, new ToBase64Transform(), CryptoStreamMode.Read))
                using (var streamReader = new StreamReader(cryptoStream))
                {                   
                    var contentType = MimeMapping.GetMimeMapping(file.VirtualFile.Name);
                    var className = ImageStyles.GetClassName(file.VirtualFile.Name);
                    var base64Content = streamReader.ReadToEnd();

                    builder.Append($".{className}{{background-image:url(data:{contentType};base64,{base64Content})}}");
                }
            }

            response.Content = builder.ToString();
            response.ContentType = "text/css";
        }
    }
}
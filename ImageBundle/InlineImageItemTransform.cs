namespace ImageBundle
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Optimization;

    public class InlineImageItemTransform : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            string ReplaceUrlWithBase64Content(Match match)
            {
                var path = match.Groups[1].Value.Trim('\'', '"');
                if (path.Contains("://"))
                {
                    return match.Value;
                }

                var physicalPath = HttpContext.Current.Server.MapPath(path);

                using (var stream = File.OpenRead(physicalPath))
                using (var cryptoStream = new CryptoStream(stream, new ToBase64Transform(), CryptoStreamMode.Read))
                using (var streamReader = new StreamReader(cryptoStream))
                {
                    var contentType = MimeMapping.GetMimeMapping(includedVirtualPath);

                    var base64Content = streamReader.ReadToEnd();

                    return $"url(data:{contentType};base64,{base64Content})";
                }
            }

            return Regex.Replace(input, "url\\(([^\\)]+)\\)", ReplaceUrlWithBase64Content);
        }
    }
}

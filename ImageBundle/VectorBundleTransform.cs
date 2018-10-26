namespace ImageBundle
{
    using System;
    using System.Linq;
    using System.Web.Optimization;
    using System.Xml.Linq;

    public class VectorBundleTransform : IBundleTransform
    {
        private static readonly XNamespace SvgNamespace = "http://www.w3.org/2000/svg";
        private static readonly string[] ExcludeAttributes = { "id", "xmlns", "version" };

        public void Process(BundleContext context, BundleResponse response)
        {
            XAttribute destinationVersionAttribute = null;
            var defsElement = new XElement(SvgNamespace + "defs");

            foreach (var file in response.Files)
            {
                var symbolElement = new XElement(SvgNamespace + "symbol");
                symbolElement.Add(new XAttribute("id", file.VirtualFile.Name));

                using (var stream = file.VirtualFile.Open())
                {                   
                    var sourceDocument = XDocument.Load(stream);

                    symbolElement.Add(sourceDocument.Root.Descendants());

                    var sourceVersionAttribute = sourceDocument.Root.Attribute("version");
                    if (sourceVersionAttribute == null)
                    {
                        throw new NotSupportedException("SVG version attribute is required to be set.");
                    }

                    if (destinationVersionAttribute == null)
                    {
                        destinationVersionAttribute = sourceVersionAttribute;
                    }
                    else if (sourceVersionAttribute.Value != destinationVersionAttribute.Value)
                    {
                        throw new NotSupportedException("Mixing SVG versions in the same bundle is not supported.");
                    }

                    var attributes = sourceDocument.Root.Attributes()
                        .Where(attribute => ExcludeAttributes.Contains(attribute.Name.LocalName) == false)
                        .ToArray();

                    symbolElement.Add(attributes);
                }

                defsElement.Add(symbolElement);
            }

            var declaration = new XDeclaration("1.0", null, null);
            var document = new XDocument(new XElement(SvgNamespace + "svg", defsElement, destinationVersionAttribute));

            response.Content = declaration + Environment.NewLine + document.ToString(SaveOptions.DisableFormatting);
            response.ContentType = "image/svg+xml";
        }
    }
}
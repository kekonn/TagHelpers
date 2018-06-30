using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kekonn.TagHelpers.Core.Assets
{
    [HtmlTargetElement("link", Attributes = LibraryAttributeName)]
    [HtmlTargetElement("link", Attributes = MinifiedAttributeName)]
    public class StylesheetAssetTagHelper : TagHelper
    {
        private const string LibraryAttributeName = "ass-library";
        private const string HrefAttributeName = "href";
        private const string MinifiedAttributeName = "ass-minified";

        [HtmlAttributeName(LibraryAttributeName)]
        public string Library { get; set; }

        [HtmlAttributeName(MinifiedAttributeName)]
        public string Minified { get; set; }

        private readonly IAssetStore _store;

        public StylesheetAssetTagHelper(IAssetStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Library))
            {
                ProcessLibrary(context,output);
            }
            else
            {
                throw new InvalidOperationException("you have to define a library to search for");
            }
        }
        private void ProcessLibrary(TagHelperContext context, TagHelperOutput output)
        {
            //search the store for the library
            var assetKey = new AssetStoreKey(Library,AssetType.Stylesheet);
            var assetDefinition = _store[assetKey];
            if (assetDefinition == null)
                throw new FileNotFoundException();

            var minified = Minified != null;

            if (minified)
            {
                output.Attributes.SetAttribute(HrefAttributeName, assetDefinition.MinifiedLocation);
            }
            else
            {
                output.Attributes.SetAttribute(HrefAttributeName, assetDefinition.AssetLocation);
            }
        }
    }
}

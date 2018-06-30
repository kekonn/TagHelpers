using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kekonn.TagHelpers.Core.Assets
{
    [HtmlTargetElement("script", Attributes = LibraryAttributeName)]
    [HtmlTargetElement("script", Attributes = MinifiedAttributeName)]
    public class ScriptAssetTagHelper : TagHelper
    {
        #region Constants
        private const string LibraryAttributeName = "ass-library";
        private const string MinifiedAttributeName = "ass-minified";
        private const string SourceAttributeName = "src";
        #endregion

        #region Attribute Properties
        [HtmlAttributeName(LibraryAttributeName)]
        public string Library { get; set; }

        [HtmlAttributeName(MinifiedAttributeName)]
        public string Minified { get; set; }
        #endregion

        private readonly IAssetStore _store;

        public ScriptAssetTagHelper(IAssetStore store)
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
            var assetKey = new AssetStoreKey(Library,AssetType.Script);
            var assetDefinition = _store[assetKey];
            if (assetDefinition == null)
                throw new FileNotFoundException();

            var minified = Minified != null;

            if (minified)
            {
                output.Attributes.SetAttribute(SourceAttributeName, assetDefinition.MinifiedLocation);
            }
            else
            {
                output.Attributes.SetAttribute(SourceAttributeName, assetDefinition.AssetLocation);
            }
        }
        
    }
}

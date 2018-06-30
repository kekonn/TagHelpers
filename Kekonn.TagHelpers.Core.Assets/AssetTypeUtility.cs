using System;
using System.Collections.Generic;
using System.Text;

namespace Kekonn.TagHelpers.Core.Assets
{
    internal static class AssetTypeUtility
    {
        internal static string AssetTypeToString(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Script:
                    return "script";
                case AssetType.Stylesheet:
                    return "stylesheet";
                case AssetType.Library:
                    return "library";
                default:
                    return string.Empty;
            }
        }
    }
}

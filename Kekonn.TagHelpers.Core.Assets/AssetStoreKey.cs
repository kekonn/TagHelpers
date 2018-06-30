using System;
using System.Collections.Generic;
using System.Text;

namespace Kekonn.TagHelpers.Core.Assets
{
    public sealed class AssetStoreKey
    {
        public string AssetName { get; private set; }
        public AssetType AssetType { get; private set; }

        public AssetStoreKey(string assetName, AssetType assetType)
        {
            AssetName = assetName ?? throw new ArgumentNullException(nameof(assetName));
            AssetName = AssetName.ToLower();
            AssetType = assetType;
        }
        
    }
}

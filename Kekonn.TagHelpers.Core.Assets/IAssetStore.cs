using System;
using System.Collections.Generic;
using System.Text;

namespace Kekonn.TagHelpers.Core.Assets
{
    public interface IAssetStore
    {
        IEnumerable<AssetDefinition> Scripts { get; }
        IEnumerable<AssetDefinition> Stylesheets { get; }
        AssetDefinition this[AssetStoreKey key] { get; }
    }
}

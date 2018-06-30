using System;
using System.Collections.Generic;
using System.Text;

namespace Kekonn.TagHelpers.Core.Assets
{
    public class AssetDefinition
    {
        public AssetType AssetType { get; private set; }
        public string AssetLocation { get; private set; }
        public string AssetName { get; private set; }

        public string SRIHash { get; set; }
        public string MinifiedLocation { get; set; }
        public bool MinifiedVersionAvailable
        {
            get
            {
                return !string.IsNullOrWhiteSpace(MinifiedLocation);
            }
        }

        public AssetDefinition(AssetType assetType, string assetLocation, string assetName)
        {
            AssetType = assetType;
            AssetName = assetName ?? throw new ArgumentNullException(nameof(assetName));
            if (AssetType == AssetType.Library)
            {
                AssetLocation = $"~/lib/{assetName}";
            } else
            {
                AssetLocation = assetLocation ?? throw new ArgumentNullException(nameof(assetLocation));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}",AssetTypeUtility.AssetTypeToString(AssetType),AssetName);
        }

        #region Equals
        public override bool Equals(object obj)
        {
            var definition = obj as AssetDefinition;
            return definition != null &&
                   AssetType == definition.AssetType &&
                   AssetLocation == definition.AssetLocation.ToLower() &&
                   AssetName == definition.AssetName.ToLower();
        }

        public override int GetHashCode()
        {
            var hashCode = 1140047375;
            hashCode = hashCode * -1521134295 + AssetType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssetLocation);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssetName);
            return hashCode;
        }
        #endregion
    }
}

using System;
using System.Linq;
using System.Collections.Generic;

namespace Kekonn.TagHelpers.Core.Assets
{
    public sealed class AssetStore : IAssetStore
    {
        private bool _isReady = false;

        private AssetStoreConfiguration _config;
        private List<AssetDefinition> _assets;

        internal void Configure(AssetStoreConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _config.ResolveLibraries();
            _assets = new List<AssetDefinition>(_config.Assets);
            _isReady = true;
        }

        #region IAssetStore

        public AssetDefinition this[AssetStoreKey key]
        {
            get
            {
                return _assets.Where(a => a.AssetType == key.AssetType && a.AssetName.Equals(key.AssetName)).Single();
            }
        }

        public IEnumerable<AssetDefinition> Scripts
        {
            get
            {
                EnsureReady();

                return _assets.Where(a => a.AssetType == AssetType.Script);
            }
        }

        public IEnumerable<AssetDefinition> Stylesheets
        {
            get
            {
                EnsureReady();

                return _assets.Where(a => a.AssetType == AssetType.Stylesheet);
            }
        }

        #endregion

        private void EnsureReady()
        {
            if (!_isReady)
                throw new InvalidOperationException("Please configure the AssetStore first.");
        }
    }
}

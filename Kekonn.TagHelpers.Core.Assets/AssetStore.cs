using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Kekonn.TagHelpers.Core.Assets
{
    public sealed class AssetStore : IAssetStore, IConfigureOptions<AssetStoreOptions>,
        IPostConfigureOptions<AssetStoreOptions>
    {
        private List<AssetDefinition> _assets;

        private AssetStoreOptions _config;
        private bool _isReady;

        public void Configure(AssetStoreOptions config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _config.ResolveLibraries();
            _assets = new List<AssetDefinition>(_config.Assets);
            _isReady = true;
        }

        public void PostConfigure(string name, AssetStoreOptions options)
        {
            throw new NotImplementedException();
        }

        private void EnsureReady()
        {
            if (!_isReady)
                throw new InvalidOperationException("Please configure the AssetStore first.");
        }

        #region IAssetStore

        public AssetDefinition this[AssetStoreKey key]
        {
            get { return _assets.Single(a => a.AssetType == key.AssetType && a.AssetName.Equals(key.AssetName)); }
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
    }
}
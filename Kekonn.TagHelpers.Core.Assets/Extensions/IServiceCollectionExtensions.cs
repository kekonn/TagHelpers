using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;

namespace Kekonn.TagHelpers.Core.Assets.Extensions
{
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Add the asset store to the services
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="config">an AssetStoreConfiguration object or null (to create a default config)</param>
        public static void AddAssetStore(this IServiceCollection serviceCollection, AssetStoreConfiguration config = null)
        {
            if (config == null)
            {
                config = new AssetStoreConfiguration(Environment.CurrentDirectory);
                config.Scan("~/lib/");
            }

            var store = new AssetStore();
            store.Configure(config);
            serviceCollection.Add(new ServiceDescriptor(typeof(IAssetStore), store));
        }

        /// <summary>
        /// The simpeler way to create the AssetStore and add it to the service collection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="options">Options for the creation and initialization of the AssetStore</param>
        public static void AddAssetStore(this IServiceCollection serviceCollection, Expression<Func<AssetStoreOptions>> options)
        {
            var storeOptions = options.Compile().Invoke();
            var root = string.IsNullOrWhiteSpace(storeOptions.Root) ? Environment.CurrentDirectory : storeOptions.Root;

            var storeConfig = new AssetStoreConfiguration(root);

            var store = new AssetStore();

            if (storeOptions.ScanOnConfigure)
            {
                storeConfig.Scan();
            }

            store.Configure(storeConfig);
            serviceCollection.Add(new ServiceDescriptor(typeof(IAssetStore), store));
        }
    }
}

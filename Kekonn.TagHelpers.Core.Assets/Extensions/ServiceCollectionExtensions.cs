using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable UnusedMember.Global

namespace Kekonn.TagHelpers.Core.Assets.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     The simpeler way to create the AssetStore and add it to the service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">Options for the creation and initialization of the AssetStore</param>
        public static void AddAssetStore(this IServiceCollection services, Action<AssetStoreOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IAssetStore, AssetStore>();
            services.Configure(options);
            services.PostConfigure(options);
        }
    }
}
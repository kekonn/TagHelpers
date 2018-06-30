using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Kekonn.TagHelpers.Core.Assets
{
    public class AssetStoreConfiguration
    {
        private const string LIB_ROOT = "~/lib";

        private Dictionary<string, AssetDefinition> _definitionDictionary = new Dictionary<string, AssetDefinition>();

        private readonly IHostingEnvironment _hostingEnvironment;

        public AssetStoreConfiguration(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        #region Libraries
        public AssetStoreConfiguration AddLibrary(string name)
        {
            return Add(new AssetDefinition(AssetType.Library, null, name));
        }

        public AssetStoreConfiguration AddLibraries(string[] libraries)
        {
            foreach (var library in libraries)
            {
                AddLibrary(library);
            }

            return this;
        }
        #endregion

        #region Scripts
        public AssetStoreConfiguration AddScript(string name, string location = null)
        {
            return Add(new AssetDefinition(AssetType.Script, location ?? $"{LIB_ROOT}/{name}/", name));
        }
        #endregion

        #region Stylesheets
        public AssetStoreConfiguration AddStylesheet(string name, string location = null)
        {
            return Add(new AssetDefinition(AssetType.Stylesheet, location ?? $"{LIB_ROOT}/{name}/", name));
        }
        #endregion

        public AssetStoreConfiguration Scan(string location = null)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                location = LIB_ROOT;
            }

            var libFolders = _hostingEnvironment.WebRootFileProvider.GetDirectoryContents(LIB_ROOT).AsParallel()
                .Where(f => f.IsDirectory && (IsFolderAssetRoot(f) != null));

            var assets = libFolders.Select(lf => CreateAssetDefinitionFromRoot(lf));

            foreach (var asset in assets)
            {
                Add(asset);
            }

            return this;
        }

        private AssetDefinition CreateAssetDefinitionFromRoot(IFileInfo folder)
        {
            var type = IsFolderAssetRoot(folder).Value; //no check is needed, since we've filtered out the nulls by now
            switch (type)
            {
                case AssetType.Script:
                    return new AssetDefinition(AssetType.Script, Path.Combine(folder.PhysicalPath, "js"), folder.Name);
                case AssetType.Stylesheet:
                    return new AssetDefinition(AssetType.Stylesheet, Path.Combine(folder.PhysicalPath, "css"), folder.Name);
                case AssetType.Library:
                    return new AssetDefinition(AssetType.Script, folder.PhysicalPath, folder.Name);
                default:
                    throw new InvalidOperationException("This should never happen unless we've created asset types that aren't handled yet.");
            }
        }

        private AssetType? IsFolderAssetRoot(IFileInfo folder)
        {
            if (!folder.IsDirectory)
                return null;

            var subfolders = Directory.GetDirectories(folder.PhysicalPath);

            if (subfolders.Any(sf => sf.Equals("dist", StringComparison.InvariantCultureIgnoreCase))) //we have a dist folder, so it is certainly a library
            {
                return AssetType.Library;
            } else if (subfolders.Any(sf => sf.Equals("js",StringComparison.InvariantCultureIgnoreCase)))
            {
                return AssetType.Script;
            } else if (subfolders.Any(sf => sf.Equals("css", StringComparison.InvariantCultureIgnoreCase)))
            {
                return AssetType.Stylesheet;
            }

            return null;
        }

        public AssetStoreConfiguration Add(AssetDefinition asset)
        {
            if (_definitionDictionary.ContainsKey(asset.ToString()))
            {
                return this; //asset was detected twice
            }
            else
            {
                _definitionDictionary.Add(asset.ToString(), asset);
            }
            return this;
        }


    }
}

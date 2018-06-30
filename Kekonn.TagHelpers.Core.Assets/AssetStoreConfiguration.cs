using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Kekonn.TagHelpers.Core.Assets
{
    public class AssetStoreConfiguration
    {
        private const string LIB_ROOT = "lib";

        private Dictionary<string, AssetDefinition> _definitionDictionary = new Dictionary<string, AssetDefinition>();

        private readonly string _wwwroot;

        internal IEnumerable<AssetDefinition> Assets
        {
            get
            {
                return _definitionDictionary.Values;
            }
        }

        public AssetStoreConfiguration(string wwwRoot)
        {
            _wwwroot = string.IsNullOrWhiteSpace(wwwRoot) ? Environment.CurrentDirectory : wwwRoot;
        }

        #region Libraries
        public void AddLibrary(string name)
        {
            Add(new AssetDefinition(AssetType.Library, null, name));
        }

        public void AddLibraries(string[] libraries)
        {
            foreach (var library in libraries)
            {
                AddLibrary(library);
            }
        }
        #endregion

        #region Scripts
        public void AddScript(string name, string location = null)
        {
            Add(new AssetDefinition(AssetType.Script, location ?? $"{LIB_ROOT}/{name}/", name));
        }
        #endregion

        #region Stylesheets
        public void AddStylesheet(string name, string location = null)
        {
            Add(new AssetDefinition(AssetType.Stylesheet, location ?? $"{LIB_ROOT}/{name}/", name));
        }
        #endregion

        public void Scan(string location = null)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                location = LIB_ROOT;
            }

            var libFolders = Directory.GetDirectories(_wwwroot).AsParallel()
                .Where(f => IsFolderAssetRoot(f) != null);

            var assets = libFolders.Select(lf => CreateAssetDefinitionFromRoot(lf));

            assets.ForAll(a => Add(a));
        }

        public void Add(AssetDefinition asset)
        {
            if (_definitionDictionary.ContainsKey(asset.ToString()))
            {
                return; //asset was detected twice
            }
            else
            {
                _definitionDictionary.Add(asset.ToString(), asset);
            }
        }
        
        #region Internals
        private AssetDefinition CreateAssetDefinitionFromRoot(string folder)
        {
            var type = IsFolderAssetRoot(folder).Value; //no check is needed, since we've filtered out the nulls by now
            var folderName = Path.GetDirectoryName(folder);

            switch (type)
            {
                case AssetType.Script:
                    return new AssetDefinition(AssetType.Script, Path.Combine(folder, "js"), folderName);
                case AssetType.Stylesheet:
                    return new AssetDefinition(AssetType.Stylesheet, Path.Combine(folder, "css"), folderName);
                case AssetType.Library:
                    if (Directory.GetDirectories(folder).Any(d => d.Equals("dist", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return new AssetDefinition(AssetType.Library, Path.Combine(folder, "dist"), folderName);
                    }
                    else
                    {
                        return new AssetDefinition(AssetType.Library, folder, folderName);
                    }
                default:
                    throw new InvalidOperationException("This should never happen unless we've created asset types that aren't handled yet.");
            }
        }

        internal void ResolveLibraries()
        {
            var libraries = _definitionDictionary.Values.Where(v => v.AssetType == AssetType.Library).AsParallel();

            void ResolveLibrary(AssetDefinition library)
            {
                var foldersInLibrary = Directory.GetDirectories(library.AssetLocation);

                var cssFolder = foldersInLibrary.SingleOrDefault(f => f.Equals("css", StringComparison.InvariantCultureIgnoreCase));
                var jsFolder = foldersInLibrary.SingleOrDefault(f => f.Equals("js", StringComparison.InvariantCultureIgnoreCase));

                if (cssFolder != null)
                {
                    AddStylesheet(library.AssetName, cssFolder);
                }

                if (jsFolder != null)
                {
                    AddScript(library.AssetName, jsFolder);
                }

                _definitionDictionary.Remove(library.ToString());
            }

            libraries.ForAll(ResolveLibrary);
        }

        private AssetType? IsFolderAssetRoot(string folder)
        {
            if (!Directory.Exists(folder))
                return null;

            var subfolders = Directory.GetDirectories(folder);

            if (subfolders.Any(sf => sf.Equals("dist", StringComparison.InvariantCultureIgnoreCase))) //we have a dist folder, so it is certainly a library
            {
                return AssetType.Library;
            }
            else if (subfolders.Any(sf => sf.Equals("js", StringComparison.InvariantCultureIgnoreCase)))
            {
                return AssetType.Script;
            }
            else if (subfolders.Any(sf => sf.Equals("css", StringComparison.InvariantCultureIgnoreCase)))
            {
                return AssetType.Stylesheet;
            }

            return null;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;

using USP.MetaFileExtension;

namespace USP.MetaAddressables
{

    public static partial class MetaAddressables
    {
        #region Types
        public class Factory
        {
            #region Properties
            public AddressableAssetGroupTemplate LocalGroupTemplate
            {
                get;
                set;
            }

            public AddressableAssetGroupTemplate ActiveGroupTemplate
            {
                get;
                set;
            }
            #endregion

            #region Methods
            public UserData Create(AssetImporter assetImporter)
            {
                // Get the asset GUID associated with the asset file path.
                string guid = AssetDatabase.AssetPathToGUID(assetImporter.assetPath);

                var settings = AddressableAssetSettingsDefaultObject.Settings;

                // Attempt to find an Addressable asset entry that is associated with the asset GUID.
                // If there is, then the asset is already Addressable.
                AddressableAssetEntry entry = settings.FindAssetEntry(guid);

                // If the asset is already Addressable, then: 
                if (entry != null)
                {
                    return new UserData(entry);
                }

                var assetData = new AssetData(guid, assetImporter.assetPath, null, false);
                var groupData = new GroupData(ActiveGroupTemplate);

                return new UserData(assetData, groupData);
            }
            #endregion
        }
        #endregion
    }
}

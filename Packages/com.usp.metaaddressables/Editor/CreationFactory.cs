using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using USP.AddressablesMemento;

namespace USP.MetaAddressables
{

    public static partial class MetaAddressables
    {
        #region Types
        public class CreationFactory : ICreationFactory
        {
            #region Properties
            public AddressableAssetSettings Settings
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

                // If the Addressables settings are valid, then:
                if (Settings != null)
                {
                    // Attempt to create data off of an existing entry in Addressables.
                    var result = UserData.Create(Settings, guid);

                    // If the result is valid, then it already exists as an entry in the Addressables system.
                    // If there already is an entry in the Addressables system, then::
                    if (result != null)
                    {
                        // Use that entry and do nothing else.
                        return result;
                    }    

                    // Otherwise, there is no entry in the Addressables system.
                }

                // Either the settings wwere invalid, or there is no entry in the Addressables system. Make one. 

                var assetData = new AssetData(guid, assetImporter.assetPath, null, false);
                var groupData = new GroupData(ActiveGroupTemplate);

                return new UserData(assetData, groupData);
            }
            #endregion
        }
        #endregion
    }
}

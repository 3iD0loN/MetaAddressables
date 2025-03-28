using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

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

                if (Settings == null)
                {
                    var assetData = new AssetData(guid, assetImporter.assetPath, null, false);
                    var groupData = new GroupData(ActiveGroupTemplate);

                    return new UserData(assetData, groupData);
                }
                
                return UserData.Create(Settings, guid);
            }
            #endregion
        }
        #endregion
    }
}

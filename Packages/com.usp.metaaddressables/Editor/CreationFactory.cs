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
                if (settings == null)
                {
                    var assetData = new AssetData(guid, assetImporter.assetPath, null, false);
                    var groupData = new GroupData(ActiveGroupTemplate);

                    return new UserData(assetData, groupData);
                }
                
                return UserData.Create(settings, guid);
            }
            #endregion
        }
        #endregion
    }
}

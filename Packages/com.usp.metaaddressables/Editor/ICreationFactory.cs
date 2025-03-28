using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace USP.MetaAddressables
{

    public static partial class MetaAddressables
    {
        #region Types
        public interface ICreationFactory
        {
            #region Methods
            UserData Create(AssetImporter assetImporter);
            #endregion
        }
        #endregion
    }
}

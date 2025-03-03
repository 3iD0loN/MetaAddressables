using System;

using UnityEngine;

using UnityEditor.AddressableAssets.Settings;

namespace USP.MetaAddressables
{
    public partial class MetaAddressables
    {
        #region Types
        [Serializable]
        public class UserData
        {
            #region Static Methods
            public static UserData Create(AddressableAssetSettings settings, string assetGuid)
            {
                if (settings == null)
                {
                    return null;
                }

                // Attempt to find an Addressable asset entry that is associated with the asset GUID.
                AddressableAssetEntry entry = settings.FindAssetEntry(assetGuid);

                // If an entry does not exist, then the asset is not already Addressable.
                // If the asset is not already Addressable, then:
                if (entry == null)
                {
                    // Return an invalid user data.
                    return null;
                }

                // Otherwise, the asset is already Addressable.

                // Create a use data item from the entry.
                return new UserData(entry);
            }
            #endregion

            #region Fields
            [SerializeField]
            private AssetData _asset;

            [SerializeField]
            private GroupData _group;
            #endregion

            #region Properties
            public AssetData Asset
            {
                get { return _asset; }
                set { _asset = value; }
            }

            public GroupData Group
            {
                get { return _group; }
                set { _group = value; }
            }
            #endregion

            #region Methods
            public UserData(AddressableAssetEntry entry) :
                this(new AssetData(entry), new GroupData(entry.parentGroup))
            {
            }

            public UserData(AssetData asset, GroupData group)
            {
                _asset = asset;
                _group = group;
            }
            #endregion
        }
        #endregion
    }
}

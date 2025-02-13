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

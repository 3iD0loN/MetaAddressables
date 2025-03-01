using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using UnityEditor.AddressableAssets.Settings;

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
        [Serializable]
        public class AssetData : ISerializationCallbackReceiver
        {
            #region Static Methods
            public static string SimplifyAddress(string address)
            {
                return Path.GetFileNameWithoutExtension(address);
            }

            public static AddressableAssetEntry CreateOrMove(AddressableAssetSettings settings,
                AddressableAssetGroup group, AssetData assetData)
            {
                return CreateOrMove(settings, assetData.Guid, group, assetData.Address, assetData.Labels);
            }

            public static AddressableAssetEntry CreateOrMove(AddressableAssetSettings settings, string guid,
                AddressableAssetGroup group, string address = null, HashSet<string> labels = null)
            {
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);

                if (address != null)
                {
                    entry.SetAddress(address);
                }

                if (labels != null || labels.Count != 0)
                {
                    entry.labels.Clear();
                    entry.labels.UnionWith(labels);
                }

                return entry;
            }
            #endregion

            #region Fields
            [SerializeField]
            string _guid;

            [SerializeField]
            private string _address;

            [SerializeField]
            private bool _readOnly;

            [SerializeField]
            private string[] _labels;

            [SerializeField]
            private bool _flaggedDuringContentUpdateRestriction = false;
            #endregion

            #region Properties
            public string Guid
            {
                get
                {
                    return _guid;
                }
            }

            public string Address
            {
                get
                {
                    return _address;
                }
                set
                {
                    _address = value;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _readOnly;
                }
                set
                {
                    _readOnly = value;
                }
            }

            public HashSet<string> Labels
            {
                get;
                private set;
            }

            public bool FlaggedDuringContentUpdateRestriction
            {
                get
                {
                    return _flaggedDuringContentUpdateRestriction;
                }
            }
            #endregion

            #region Methods
            #region Constructors
            public AssetData(AddressableAssetEntry entry) :
                this(entry.guid, entry.address, entry.labels, entry.ReadOnly)
            {
            }
            
            public AssetData(string guid, string address, HashSet<string> labels, bool readOnly)
            {
                _guid = guid;

                _address = address;

                _labels = new string[0];
                Labels = labels ?? new HashSet<string>();

                _readOnly = readOnly;
            }

            #endregion

            #region ISerializationCallbackReceiver
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                _labels = new string[Labels.Count];
                Labels.CopyTo(_labels);
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                Labels = new HashSet<string>(_labels);
            }
            #endregion
            #endregion
        }
        #endregion
    }
}

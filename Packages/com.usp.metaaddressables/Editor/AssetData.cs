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
        [Serializable]
        public class AssetData : ISerializationCallbackReceiver
        {
            #region Static Methods
            public static string SimplifyAddress(string address)
            {
                return Path.GetFileNameWithoutExtension(address);
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
            public AssetData(string guid, string address, bool readOnly)
            {
                _guid = guid;

                _address = address;

                _readOnly = readOnly;

                _labels = new string[0];
                Labels = new HashSet<string>();
            }

            public AssetData(AddressableAssetEntry entry)
            {
                _guid = entry.guid;

                _address = entry.address;

                _readOnly = entry.ReadOnly;

                _labels = new string[0];
                Labels = entry.labels;
            }
            #endregion

            #region ISerializationCallbackReceiver
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
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

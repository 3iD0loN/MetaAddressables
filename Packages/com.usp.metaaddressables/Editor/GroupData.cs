using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;
using static USP.MetaAddressables.MetaAddressables;


namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
        [Serializable]
        public class GroupData : IEqualityComparer<GroupData>, ISerializationCallbackReceiver
        {
            #region Types
            public static class Comparer
            {
                #region Static Methods
                public static bool CompareName(GroupData leftHand, GroupData rightHand)
                {
                    var lhs = (object)leftHand;
                    var rhs = (object)rightHand;

                    if (lhs == rhs)
                    {
                        return true;
                    }

                    if (rhs == null || lhs == null)
                    {
                        return false;
                    }

                    return StringComparer.Ordinal.Equals(leftHand.Name, rightHand.Name);
                }

                public static bool CompareNameAndHash(GroupData leftHand, GroupData rightHand)
                {
                    var lhs = (object)leftHand;
                    var rhs = (object)rightHand;

                    if (lhs == rhs)
                    {
                        return true;
                    }

                    if (rhs == null || lhs == null)
                    {
                        return false;
                    }

                    return StringComparer.Ordinal.Equals(leftHand.Name, rightHand.Name) &&
                        ObjectComparer.CompareHash(leftHand, rightHand);
                }

                public static bool CompareGuid(GroupData leftHand, GroupData rightHand)
                {
                    var lhs = (object)leftHand;
                    var rhs = (object)rightHand;

                    if (lhs == rhs)
                    {
                        return true;
                    }

                    if (rhs == null || lhs == null)
                    {
                        return false;
                    }

                    return StringComparer.Ordinal.Equals(leftHand.Guid, rightHand.Guid);
                }
                #endregion

                #region Static Fields
                public static readonly GenericComparer<GroupData> ByName = new GenericComparer<GroupData>(CompareName);

                public static readonly GenericComparer<GroupData> ByHash = new GenericComparer<GroupData>(ObjectComparer.CompareHash);

                public static readonly GenericComparer<GroupData> ByNameAndHash = new GenericComparer<GroupData>(CompareNameAndHash);

                public static readonly GenericComparer<GroupData> ByGuid = new GenericComparer<GroupData>(CompareGuid);
                #endregion
            }
            #endregion

            #region Static Methods
            #region Create
            public static AddressableAssetGroup Create(AddressableAssetSettings settings, GroupData groupData)
            {
                List<AddressableAssetGroupSchema> schemas = Create(groupData.SchemaData);

                return settings.CreateGroup(groupData.Name, false, groupData.IsReadOnly, false, schemas);
            }

            private static GroupSchemaData Create(AddressableAssetGroupSchema groupSchema)
            {
                if (groupSchema is BundledAssetGroupSchema bundledAssetGroupScema)
                {
                    return new BundledAssetGroupSchemaData(bundledAssetGroupScema);
                }
                else if (groupSchema is ContentUpdateGroupSchema contentUpdateGroupSchemaData)
                {
                    return new ContentUpdateGroupSchemaData(contentUpdateGroupSchemaData);
                }

                return null;
            }

            private static Dictionary<Type, GroupSchemaData> Create(List<AddressableAssetGroupSchema> groupSchemas)
            {
                var result = new Dictionary<Type, GroupSchemaData>(groupSchemas.Count);
                foreach (AddressableAssetGroupSchema groupSchema in groupSchemas)
                {
                    var data = Create(groupSchema);
                    result.Add(data.GetType(), data);
                }

                return result;
            }

            private static AddressableAssetGroupSchema Create(GroupSchemaData groupSchemaData)
            {
                if (groupSchemaData is BundledAssetGroupSchemaData bundledAssetGroupScema)
                {
                    var settings = AddressableAssetSettingsDefaultObject.Settings;

                    var groupSchema = new BundledAssetGroupSchema();

                    groupSchema.InternalBundleIdMode = bundledAssetGroupScema.InternalBundleIdMode;
                    groupSchema.Compression = bundledAssetGroupScema.Compression;
                    groupSchema.IncludeAddressInCatalog = bundledAssetGroupScema.IncludeAddressInCatalog;
                    groupSchema.IncludeGUIDInCatalog = bundledAssetGroupScema.IncludeGUIDInCatalog;
                    groupSchema.IncludeLabelsInCatalog = bundledAssetGroupScema.IncludeLabelsInCatalog;
                    groupSchema.InternalIdNamingMode = bundledAssetGroupScema.InternalIdNamingMode;
                    groupSchema.AssetBundledCacheClearBehavior = bundledAssetGroupScema.CacheClearBehavior;
                    groupSchema.IncludeInBuild = bundledAssetGroupScema.IncludeInBuild;
                    groupSchema.BundledAssetProviderType = bundledAssetGroupScema.AssetBundleProviderType;
                    groupSchema.ForceUniqueProvider = bundledAssetGroupScema.ForceUniqueProvider;
                    groupSchema.UseAssetBundleCache = bundledAssetGroupScema.UseAssetBundleCache;
                    groupSchema.UseAssetBundleCrc = bundledAssetGroupScema.UseAssetBundleCrc;
                    groupSchema.UseAssetBundleCrcForCachedBundles = bundledAssetGroupScema.UseAssetBundleCrcForCachedBundles;
                    groupSchema.UseUnityWebRequestForLocalBundles = bundledAssetGroupScema.UseUWRForLocalBundles;
                    groupSchema.Timeout = bundledAssetGroupScema.Timeout;
                    groupSchema.ChunkedTransfer = bundledAssetGroupScema.ChunkedTransfer;
                    groupSchema.RedirectLimit = bundledAssetGroupScema.RedirectLimit;
                    groupSchema.RetryCount = bundledAssetGroupScema.RetryCount;
                    groupSchema.BuildPath.SetVariableById(settings, bundledAssetGroupScema.BuildPath.Id);
                    groupSchema.LoadPath.SetVariableById(settings, bundledAssetGroupScema.LoadPath.Id);
                    groupSchema.BundleMode = bundledAssetGroupScema.BundleMode;
                    groupSchema.AssetBundleProviderType = bundledAssetGroupScema.AssetBundleProviderType;
                    groupSchema.UseDefaultSchemaSettings = bundledAssetGroupScema.UseDefaultSchemaSettings;
                    groupSchema.SelectedPathPairIndex = bundledAssetGroupScema.SelectedPathPairIndex;
                    groupSchema.BundleNaming = bundledAssetGroupScema.BundleNaming;
                    groupSchema.AssetLoadMode = bundledAssetGroupScema.AssetLoadMode;

                    return groupSchema;
                }
                else if (groupSchemaData is ContentUpdateGroupSchemaData contentUpdateGroupSchema)
                {
                    var groupSchema = new ContentUpdateGroupSchema();
                    groupSchema.StaticContent = contentUpdateGroupSchema.StaticContent;

                    return groupSchema;
                }

                return default;
            }

            private static List<AddressableAssetGroupSchema> Create(Dictionary<Type, GroupSchemaData> groupSchemaData)
            {
                var result = new List<AddressableAssetGroupSchema>(groupSchemaData.Count);
                foreach (GroupSchemaData schemaData in groupSchemaData.Values)
                {
                    AddressableAssetGroupSchema schema = Create(schemaData);
                    result.Add(schema);
                }

                return result;
            }
            #endregion

            #region Equality operators
            public static bool operator ==(GroupData leftHand, GroupData rightHand)
            {
                return ObjectComparer.CompareHash(leftHand, rightHand);
            }

            public static bool operator !=(GroupData lhs, GroupData rhs)
            {
                return !(lhs == rhs);
            }
            #endregion
            #endregion

            #region Fields
            [SerializeField]
            private string _guid;

            [SerializeField]
            private bool _readOnly;

            [SerializeReference]
            private GroupSchemaData[] _schemaData;
            #endregion

            #region Properties
            public string Name { get; }

            public string Guid => _guid;

            public bool IsReadOnly => _readOnly;

            public Dictionary<Type, GroupSchemaData> SchemaData
            {
                get;
                private set;
            }
            #endregion

            #region Methods
            #region Constructors
            public GroupData(AddressableAssetGroupTemplate groupTemplate) :
                this(groupTemplate.Name, string.Empty, false, groupTemplate.SchemaObjects)
            {
            }

            public GroupData(AddressableAssetGroup group) :
                this(group.Name, group.Guid, group.ReadOnly, group.Schemas)
            {
            }

            private GroupData(string name, string guid, bool readOnly, List<AddressableAssetGroupSchema> groupSchemas) :
                this(name, guid, readOnly, Create(groupSchemas))
            {
            }

            private GroupData(string name, string guid, bool readOnly, Dictionary<Type, GroupSchemaData> groupSchemaData)
            {
                Name = name;

                _guid = guid;

                _readOnly = readOnly;

                _schemaData = new GroupSchemaData[0];
                SchemaData = groupSchemaData;
            }
            #endregion

            #region ISerializationCallbackReceiver
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                _schemaData = new GroupSchemaData[SchemaData.Count];
                SchemaData.Values.CopyTo(_schemaData, 0);
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                var result = new Dictionary<Type, GroupSchemaData>(_schemaData.Length);
                foreach (GroupSchemaData groupSchema in _schemaData)
                {
                    result.Add(groupSchema.GetType(), groupSchema);
                }

                SchemaData = result;
            }
            #endregion

            public override int GetHashCode()
            {
                int result = _readOnly.GetHashCode();

                foreach (var schema in SchemaData)
                {
                    int hash = schema.GetHashCode();
                    result = result * 31 ^ hash;
                }

                return result;
            }

            public override bool Equals(object other)
            {
                if (other is not GroupData group)
                {
                    return false;
                }

                return Equals(this, group);
            }

            public int GetHashCode(GroupData obj)
            {
                return obj.GetHashCode();
            }

            public bool Equals(GroupData lhs, GroupData rhs)
            {
                return lhs == rhs;
            }

            public override string ToString()
            {
                return Name;
            }

            public T GetSchemaData<T>() 
                where T : GroupSchemaData
            {
                var type = typeof(T);
                bool found = SchemaData.TryGetValue(type, out GroupSchemaData value);

                if (found && value is T schemaData)
                {
                    return schemaData;
                }

                return default;
            }
            #endregion
        }
        #endregion
    }
}

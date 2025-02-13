using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;


namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
        [Serializable]
        public class GroupData : IEqualityComparer<GroupData>
        {
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

            private static List<GroupSchemaData> Create(List<AddressableAssetGroupSchema> groupSchemas)
            {
                var result = new List<GroupSchemaData>(groupSchemas.Count);
                foreach (AddressableAssetGroupSchema groupSchema in groupSchemas)
                {
                    var data = Create(groupSchema);
                    result.Add(data);
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

            private static List<AddressableAssetGroupSchema> Create(List<GroupSchemaData> groupSchemaData)
            {
                var result = new List<AddressableAssetGroupSchema>(groupSchemaData.Count);
                foreach (GroupSchemaData schemaData in groupSchemaData)
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

                return leftHand.GetHashCode() == rightHand.GetHashCode();
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
            private List<GroupSchemaData> _schemaData;
            #endregion

            #region Properties
            public string Name { get; }

            public string Guid => _guid;

            public bool IsReadOnly => _readOnly;

            public List<GroupSchemaData> SchemaData => _schemaData;
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

            private GroupData(string name, string guid, bool readOnly, List<GroupSchemaData> groupSchemaData)
            {
                Name = name;

                _guid = guid;

                _readOnly = readOnly;

                _schemaData = groupSchemaData;
            }
            #endregion

            public override int GetHashCode()
            {
                int result = _readOnly.GetHashCode();

                foreach (var schema in _schemaData)
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

                return this == group;
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
            #endregion
        }
        #endregion
    }
}

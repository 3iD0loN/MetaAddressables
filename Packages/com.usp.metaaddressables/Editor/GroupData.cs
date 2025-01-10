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
        public class GroupData
        {
            #region Static Methods
            private static IGroupSchemaData CreateSchemaData(AddressableAssetGroupSchema groupSchema)
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

            private static AddressableAssetGroupSchema CreateSchema(IGroupSchemaData groupSchemaData)
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

                return null;
            }
            #endregion

            #region Fields
            [SerializeField]
            private string _guid;

            [SerializeField]
            private bool _readOnly;

            [SerializeReference]
            private List<IGroupSchemaData> _schemaData;
            #endregion

            #region Properties
            public string Guid
            {
                get
                {
                    return _guid;
                }
            }

            public bool ReadOnly
            {
                get
                {
                    return _readOnly;
                }
            }

            public List<IGroupSchemaData> SchemaData
            {
                get
                {
                    return _schemaData;
                }
            }
            #endregion

            #region Methods
            public GroupData(AddressableAssetGroupTemplate groupTemplate) :
                this(string.Empty, false, groupTemplate.SchemaObjects)
            {
            }

            public GroupData(AddressableAssetGroup group) :
                this(group.Guid, group.ReadOnly, group.Schemas)
            { }

            private GroupData(string guid, bool readOnly, List<AddressableAssetGroupSchema> groupSchemas)
            {
                _guid = guid;

                _readOnly = readOnly;

                _schemaData = new List<IGroupSchemaData>();
                foreach (AddressableAssetGroupSchema groupSchema in groupSchemas)
                {
                    var data = CreateSchemaData(groupSchema);
                    _schemaData.Add(data);
                }
            }

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
            public List<AddressableAssetGroupSchema> CreateSchemas()
            {
                var result = new List<AddressableAssetGroupSchema>(SchemaData.Count);
                foreach (IGroupSchemaData schemaData in SchemaData)
                {
                    AddressableAssetGroupSchema schema = CreateSchema(schemaData);
                    result.Add(schema);
                }

                return result;
            }
            #endregion
        }
        #endregion
    }
}

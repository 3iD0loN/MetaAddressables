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
        public abstract class GroupSchemaData : IEqualityComparer<GroupSchemaData>
        {
            #region Static Methods
            #region Create
            public static GroupSchemaData Create(AddressableAssetGroupSchema groupSchema)
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

            public static Dictionary<Type, GroupSchemaData> Create(List<AddressableAssetGroupSchema> groupSchemas)
            {
                var result = new Dictionary<Type, GroupSchemaData>(groupSchemas.Count);
                foreach (AddressableAssetGroupSchema groupSchema in groupSchemas)
                {
                    var data = Create(groupSchema);
                    result.Add(data.GetType(), data);
                }

                return result;
            }

            public static AddressableAssetGroupSchema Create(GroupSchemaData groupSchemaData)
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

            public static List<AddressableAssetGroupSchema> Create(Dictionary<Type, GroupSchemaData> groupSchemaData)
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

            public static bool operator ==(GroupSchemaData leftHand, GroupSchemaData rightHand)
            {
                return ObjectComparer.CompareHash(leftHand, rightHand.GetHashCode());
            }

            public static bool operator !=(GroupSchemaData lhs, GroupSchemaData rhs)
            {
                return !(lhs == rhs);
            }
            #endregion

            #region Methods
            public abstract override int GetHashCode();

            public override bool Equals(object other)
            {
                if (other is not GroupSchemaData schemaData)
                {
                    return false;
                }

                return Equals(this, schemaData);
            }

            public int GetHashCode(GroupSchemaData obj)
            {
                return obj.GetHashCode();
            }

            public bool Equals(GroupSchemaData lhs, GroupSchemaData rhs)
            {
                return lhs == rhs;
            }
            #endregion
        }
        #endregion
    }
}

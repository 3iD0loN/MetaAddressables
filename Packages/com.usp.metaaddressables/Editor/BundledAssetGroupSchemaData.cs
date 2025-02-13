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
        public class BundledAssetGroupSchemaData : GroupSchemaData
        {
            #region Fields
            [SerializeField]
            private BundleInternalIdMode _internalBundleIdMode = BundleInternalIdMode.GroupGuidProjectIdHash;

            [SerializeField]
            private BundleCompressionMode _compression = BundleCompressionMode.LZ4;

            [SerializeField]
            private bool _includeAddressInCatalog = true;

            [SerializeField]
            private bool _includeGUIDInCatalog = true;

            [SerializeField]
            private bool _includeLabelsInCatalog = true;

            [SerializeField]
            private AssetNamingMode _internalIdNamingMode = AssetNamingMode.FullPath;

            [SerializeField]
            private CacheClearBehavior _cacheClearBehavior = CacheClearBehavior.ClearWhenSpaceIsNeededInCache;

            [SerializeField]
            private bool _includeInBuild = true;

            [SerializeField]
            [SerializedTypeRestriction(type = typeof(IResourceProvider))]
            private SerializedType _bundledAssetProviderType;

            [SerializeField]
            private bool _forceUniqueProvider = false;

            [SerializeField]
            private bool _useAssetBundleCache = true;

            [SerializeField]
            private bool _useAssetBundleCrc = true;

            [SerializeField]
            private bool _useAssetBundleCrcForCachedBundles = true;

            [SerializeField]
            private bool _useUWRForLocalBundles = false;

            [SerializeField]
            [Min(0)]
            private int _timeout;

            [SerializeField]
            private bool _chunkedTransfer;

            [SerializeField]
            [Range(-1, 128)]
            private int _redirectLimit = -1;

            [SerializeField]
            [Range(0, 128)]
            private int _retryCount = 0;

            [SerializeField]
            private ProfileValueReference _buildPath = new ProfileValueReference();

            [SerializeField]
            private ProfileValueReference _loadPath = new ProfileValueReference();

            [SerializeField]
            private BundlePackingMode _bundleMode = BundlePackingMode.PackTogether;

            [SerializeField]
            [SerializedTypeRestriction(type = typeof(IResourceProvider))]
            private SerializedType _assetBundleProviderType;

            [SerializeField]
            private bool _useDefaultSchemaSettings;

            [SerializeField]
            private int _selectedPathPairIndex;

            [SerializeField]
            private BundleNamingStyle _bundleNaming;

            [SerializeField]
            private AssetLoadMode _assetLoadMode;
            #endregion

            #region Properties
            public BundleInternalIdMode InternalBundleIdMode
            {
                get
                {
                    return _internalBundleIdMode;
                }
            }

            public BundleCompressionMode Compression
            {
                get
                {
                    return _compression;
                }
            }

            public bool IncludeAddressInCatalog
            {
                get
                {
                    return _includeAddressInCatalog;
                }
            }

            public bool IncludeGUIDInCatalog
            {
                get
                {
                    return _includeGUIDInCatalog;
                }
            }

            public bool IncludeLabelsInCatalog
            {
                get
                {
                    return _includeLabelsInCatalog;
                }
            }

            public AssetNamingMode InternalIdNamingMode
            {
                get
                {
                    return _internalIdNamingMode;
                }
            }

            public CacheClearBehavior CacheClearBehavior
            {
                get
                {
                    return _cacheClearBehavior;
                }
            }

            public bool IncludeInBuild
            {
                get
                {
                    return _includeInBuild;
                }
            }

            public SerializedType BundledAssetProviderType
            {
                get
                {
                    return _bundledAssetProviderType;
                }
            }

            public bool ForceUniqueProvider
            {
                get
                {
                    return _forceUniqueProvider;
                }
            }

            public bool UseAssetBundleCache
            {
                get
                {
                    return _useAssetBundleCache;
                }
            }

            public bool UseAssetBundleCrc
            {
                get
                {
                    return _useAssetBundleCrc;
                }
            }

            public bool UseAssetBundleCrcForCachedBundles
            {
                get
                {
                    return _useAssetBundleCrcForCachedBundles;
                }
            }

            public bool UseUWRForLocalBundles
            {
                get
                {
                    return _useUWRForLocalBundles;
                }
            }

            public int Timeout
            {
                get
                {
                    return _timeout;
                }
            }

            public bool ChunkedTransfer
            {
                get
                {
                    return _chunkedTransfer;
                }
            }

            public int RedirectLimit
            {
                get
                {
                    return _redirectLimit;
                }
            }

            public int RetryCount
            {
                get
                {
                    return _retryCount;
                }
            }

            public ProfileValueReference BuildPath
            {
                get
                {
                    return _buildPath;
                }
            }

            public ProfileValueReference LoadPath
            {
                get
                {
                    return _loadPath;
                }
            }

            public BundlePackingMode BundleMode
            {
                get
                {
                    return _bundleMode;
                }
            }

            public SerializedType AssetBundleProviderType
            {
                get
                {
                    return _assetBundleProviderType;
                }
            }

            public bool UseDefaultSchemaSettings
            {
                get
                {
                    return _useDefaultSchemaSettings;
                }
            }

            public int SelectedPathPairIndex
            {
                get
                {
                    return _selectedPathPairIndex;
                }
            }

            public BundleNamingStyle BundleNaming
            {
                get
                {
                    return _bundleNaming;
                }
            }

            public AssetLoadMode AssetLoadMode
            {
                get
                {
                    return _assetLoadMode;
                }
            }
            #endregion

            #region Methods
            public BundledAssetGroupSchemaData(BundledAssetGroupSchema schema)
            {
                _internalBundleIdMode = schema.InternalBundleIdMode;
                _compression = schema.Compression;
                _includeAddressInCatalog = schema.IncludeAddressInCatalog;
                _includeGUIDInCatalog = schema.IncludeGUIDInCatalog;
                _includeLabelsInCatalog = schema.IncludeLabelsInCatalog;
                _internalIdNamingMode = schema.InternalIdNamingMode;
                _cacheClearBehavior = schema.AssetBundledCacheClearBehavior;
                _includeInBuild = schema.IncludeInBuild;
                _bundledAssetProviderType = schema.AssetBundleProviderType;
                _forceUniqueProvider = schema.ForceUniqueProvider;
                _useAssetBundleCache = schema.UseAssetBundleCache;
                _useAssetBundleCrc = schema.UseAssetBundleCrc;
                _useAssetBundleCrcForCachedBundles = schema.UseAssetBundleCrcForCachedBundles;
                _useUWRForLocalBundles = schema.UseUnityWebRequestForLocalBundles;
                _timeout = schema.Timeout;
                _chunkedTransfer = schema.ChunkedTransfer;
                _redirectLimit = schema.RedirectLimit;
                _retryCount = schema.RetryCount;
                _buildPath = schema.BuildPath;
                _loadPath = schema.LoadPath;
                _bundleMode = schema.BundleMode;
                _assetBundleProviderType = schema.AssetBundleProviderType;
                _useDefaultSchemaSettings = schema.UseDefaultSchemaSettings;
                _selectedPathPairIndex = schema.SelectedPathPairIndex;
                _bundleNaming = schema.BundleNaming;
                _assetLoadMode = schema.AssetLoadMode;
            }

            public override int GetHashCode()
            {
                int result = _internalBundleIdMode.GetHashCode();
                result = result * 31 ^ _compression.GetHashCode();
                result = result * 31 ^ _includeAddressInCatalog.GetHashCode();
                result = result * 31 ^ _includeGUIDInCatalog.GetHashCode();
                result = result * 31 ^ _includeLabelsInCatalog.GetHashCode();
                result = result * 31 ^ _internalIdNamingMode.GetHashCode();
                result = result * 31 ^ _cacheClearBehavior.GetHashCode();
                result = result * 31 ^ _includeInBuild.GetHashCode();
                result = result * 31 ^ _bundledAssetProviderType.Value.GetHashCode();
                result = result * 31 ^ _forceUniqueProvider.GetHashCode();
                result = result * 31 ^ _useAssetBundleCache.GetHashCode();
                result = result * 31 ^ _useAssetBundleCrc.GetHashCode();
                result = result * 31 ^ _useAssetBundleCrcForCachedBundles.GetHashCode();
                result = result * 31 ^ _useUWRForLocalBundles.GetHashCode();
                result = result * 31 ^ _timeout.GetHashCode();
                result = result * 31 ^ _chunkedTransfer.GetHashCode();
                result = result * 31 ^ _redirectLimit.GetHashCode();
                result = result * 31 ^ _retryCount.GetHashCode();
                result = result * 31 ^ _buildPath.Id.GetHashCode();
                result = result * 31 ^ _loadPath.Id.GetHashCode();
                result = result * 31 ^ _bundleMode.GetHashCode();
                result = result * 31 ^ _assetBundleProviderType.Value.GetHashCode();
                result = result * 31 ^ _useDefaultSchemaSettings.GetHashCode();
                result = result * 31 ^ _selectedPathPairIndex.GetHashCode();
                result = result * 31 ^ _bundleNaming.GetHashCode();
                result = result * 31 ^ _assetLoadMode.GetHashCode();

                return result;
            }
            #endregion
        }
        #endregion
    }
}

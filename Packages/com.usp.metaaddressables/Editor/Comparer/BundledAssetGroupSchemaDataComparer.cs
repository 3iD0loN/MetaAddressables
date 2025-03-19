using System; 
using static USP.MetaAddressables.MetaAddressables;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace USP.MetaAddressables
{
    public class BundledAssetGroupSchemaDataComparer : PropertyComparer<MetaAddressables.BundledAssetGroupSchemaData>
    {
        public BundledAssetGroupSchemaDataComparer() :
            base(new PropertyComparerPair(x => (x as BundledAssetGroupSchemaData).InternalBundleIdMode),
                new PropertyComparerPair(x => x.Compression),
                new PropertyComparerPair(x => x.IncludeAddressInCatalog),
                new PropertyComparerPair(x => x.IncludeGUIDInCatalog),
                new PropertyComparerPair(x => x.IncludeLabelsInCatalog),
                new PropertyComparerPair(x => x.InternalIdNamingMode),
                new PropertyComparerPair(x => x.CacheClearBehavior),
                new PropertyComparerPair(x => x.IncludeInBuild),
                new PropertyComparerPair(x => x.BundledAssetProviderType.Value),
                new PropertyComparerPair(x => x.ForceUniqueProvider),
                new PropertyComparerPair(x => x.UseAssetBundleCache),
                new PropertyComparerPair(x => x.UseAssetBundleCrc),
                new PropertyComparerPair(x => x.UseAssetBundleCrcForCachedBundles),
                new PropertyComparerPair(x => x.UseUWRForLocalBundles),
                new PropertyComparerPair(x => x.Timeout),
                new PropertyComparerPair(x => x.ChunkedTransfer),
                new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.RedirectLimit),
                new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.RetryCount),
                new PropertyComparerPair<BundledAssetGroupSchemaData, string>(x => x.BuildPath.Id, StringComparer.Ordinal),
                new PropertyComparerPair<BundledAssetGroupSchemaData, string>(x => x.LoadPath.Id, StringComparer.Ordinal),
                new PropertyComparerPair<BundledAssetGroupSchemaData, BundlePackingMode>(x => x.BundleMode),
                new PropertyComparerPair<BundledAssetGroupSchemaData, Type>(x => x.AssetBundleProviderType.Value),
                new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.UseDefaultSchemaSettings),
                new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.SelectedPathPairIndex),
                new PropertyComparerPair<BundledAssetGroupSchemaData, BundleNamingStyle>(x => x.BundleNaming),
                new PropertyComparerPair<BundledAssetGroupSchemaData, AssetLoadMode>(x => x.AssetLoadMode))
        /*/
        base(new PropertyComparerPair<BundledAssetGroupSchemaData, BundleInternalIdMode>(x => x.InternalBundleIdMode),
            new PropertyComparerPair<BundledAssetGroupSchemaData, BundleCompressionMode>(x => x.Compression),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.IncludeAddressInCatalog),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.IncludeGUIDInCatalog),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.IncludeLabelsInCatalog),
            new PropertyComparerPair<BundledAssetGroupSchemaData, AssetNamingMode>(x => x.InternalIdNamingMode),
            new PropertyComparerPair<BundledAssetGroupSchemaData, CacheClearBehavior>(x => x.CacheClearBehavior),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.IncludeInBuild),
            new PropertyComparerPair<BundledAssetGroupSchemaData, Type>(x => x.BundledAssetProviderType.Value),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.ForceUniqueProvider),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.UseAssetBundleCache),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.UseAssetBundleCrc),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.UseAssetBundleCrcForCachedBundles),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.UseUWRForLocalBundles),
            new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.Timeout),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.ChunkedTransfer),
            new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.RedirectLimit),
            new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.RetryCount),
            new PropertyComparerPair<BundledAssetGroupSchemaData, string>(x => x.BuildPath.Id, StringComparer.Ordinal),
            new PropertyComparerPair<BundledAssetGroupSchemaData, string>(x => x.LoadPath.Id, StringComparer.Ordinal),
            new PropertyComparerPair<BundledAssetGroupSchemaData, BundlePackingMode>(x => x.BundleMode),
            new PropertyComparerPair<BundledAssetGroupSchemaData, Type>(x => x.AssetBundleProviderType.Value),
            new PropertyComparerPair<BundledAssetGroupSchemaData, bool>(x => x.UseDefaultSchemaSettings),
            new PropertyComparerPair<BundledAssetGroupSchemaData, int>(x => x.SelectedPathPairIndex),
            new PropertyComparerPair<BundledAssetGroupSchemaData, BundleNamingStyle>(x => x.BundleNaming),
            new PropertyComparerPair<BundledAssetGroupSchemaData, AssetLoadMode>(x => x.AssetLoadMode))
        //*/
        {
        }
    }
}

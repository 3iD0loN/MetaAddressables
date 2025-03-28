namespace USP.MetaAddressables
{
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine.ResourceManagement.Util;

    public class ProfileValueReferenceComparer : PropertyComparer<ProfileValueReference>
    {
        public ProfileValueReferenceComparer() :
            base((x => x.Id, ObjectComparer.Default))
        {
        }
    }

    public class SerializedTypeComparer : PropertyComparer<SerializedType>
    {
        public SerializedTypeComparer() :
            base((x => x.Value, ObjectComparer.Default))
        {
        }
    }

    public class BundledAssetGroupSchemaDataComparer : PropertyComparer<MetaAddressables.BundledAssetGroupSchemaData>
    {
        public BundledAssetGroupSchemaDataComparer() :
            base((x => x.InternalBundleIdMode, ObjectComparer.Default),
                (x => x.Compression, ObjectComparer.Default),
                (x => x.IncludeAddressInCatalog, ObjectComparer.Default),
                (x => x.IncludeGUIDInCatalog, ObjectComparer.Default),
                (x => x.IncludeLabelsInCatalog, ObjectComparer.Default),
                (x => x.InternalIdNamingMode, ObjectComparer.Default),
                (x => x.CacheClearBehavior, ObjectComparer.Default),
                (x => x.IncludeInBuild, ObjectComparer.Default),
                (x => x.BundledAssetProviderType, new SerializedTypeComparer()),
                (x => x.ForceUniqueProvider, ObjectComparer.Default),
                (x => x.UseAssetBundleCache, ObjectComparer.Default),
                (x => x.UseAssetBundleCrc, ObjectComparer.Default),
                (x => x.UseAssetBundleCrcForCachedBundles, ObjectComparer.Default),
                (x => x.UseUWRForLocalBundles, ObjectComparer.Default),
                (x => x.Timeout, ObjectComparer.Default),
                (x => x.ChunkedTransfer, ObjectComparer.Default),
                (x => x.RedirectLimit, ObjectComparer.Default),
                (x => x.RetryCount, ObjectComparer.Default),
                (x => x.BuildPath, new ProfileValueReferenceComparer()),
                (x => x.LoadPath, new ProfileValueReferenceComparer()),
                (x => x.BundleMode, ObjectComparer.Default),
                (x => x.AssetBundleProviderType, new SerializedTypeComparer()),
                (x => x.UseDefaultSchemaSettings, ObjectComparer.Default),
                (x => x.SelectedPathPairIndex, ObjectComparer.Default),
                (x => x.BundleNaming, ObjectComparer.Default),
                (x => x.AssetLoadMode, ObjectComparer.Default))
        {
        }
    }
}

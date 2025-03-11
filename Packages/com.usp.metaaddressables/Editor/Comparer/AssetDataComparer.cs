namespace USP.MetaAddressables
{
    public class AssetDataComparer : PropertyComparer<MetaAddressables.AssetData>
    {
        public AssetDataComparer() :
            base((x => x.Address, StringComparer.Ordinal),
                (x => x.IsReadOnly, ObjectComparer.Default),
                (x => x.Labels, new EnumerableComparer<string>(StringComparer.Ordinal)))
        {
        }
    }
}

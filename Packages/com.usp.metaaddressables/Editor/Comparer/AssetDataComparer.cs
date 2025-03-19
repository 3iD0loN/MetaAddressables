using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class AssetDataComparer : PropertyComparer//<MetaAddressables.AssetData>
    {
        public AssetDataComparer() :
            base(new PropertyComparerPair(x => (x as MetaAddressables.AssetData).Address, StringComparer.Ordinal),
                new PropertyComparerPair(x => (x as MetaAddressables.AssetData).IsReadOnly, ObjectComparer<bool>.Default),
                new PropertyComparerPair(x => (x as MetaAddressables.AssetData).Labels, new EnumerableComparer<string, StringComparer>(StringComparer.Ordinal)))
            /*/
            base(new PropertyComparerPair<MetaAddressables.AssetData, string>(x => x.Address, StringComparer.Ordinal),
                new PropertyComparerPair<MetaAddressables.AssetData, bool>(x => x.IsReadOnly, ObjectComparer<bool>.Default),
                new PropertyComparerPair<MetaAddressables.AssetData, IEnumerable<string>>(x => x.Labels, new EnumerableComparer<string, StringComparer>(StringComparer.Ordinal)))
            //*/
        {
        }
    }
}

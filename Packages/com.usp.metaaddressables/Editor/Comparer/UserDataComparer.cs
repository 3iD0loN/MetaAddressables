namespace USP.MetaAddressables
{
    public class UserDataComparer : PropertyComparer<MetaAddressables.UserData>
    {
        public UserDataComparer() :
            base(new PropertyComparerPair<MetaAddressables.UserData, MetaAddressables.AssetData>(x => x.Asset, new AssetDataComparer()),
                new PropertyComparerPair<MetaAddressables.UserData, MetaAddressables.GroupData>(x => x.Group, new GroupDataComparer()))
        {
        }
    }
}

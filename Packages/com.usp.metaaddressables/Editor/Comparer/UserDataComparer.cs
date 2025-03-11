namespace USP.MetaAddressables
{
    public class UserDataComparer : PropertyComparer<MetaAddressables.UserData>
    {
        public UserDataComparer() :
            base((x => x.Asset, new AssetDataComparer()),
                (x => x.Group, new GroupDataComparer()))
        {
        }
    }
}

namespace USP.MetaAddressables
{
    public class ContentUpdateGroupSchemaDataComparer : PropertyComparer<MetaAddressables.ContentUpdateGroupSchemaData>
    {
        public ContentUpdateGroupSchemaDataComparer() :
            base(new PropertyComparerPair<MetaAddressables.ContentUpdateGroupSchemaData, bool>(x => x.StaticContent, ObjectComparer<bool>.Default))
        {
        }
    }
}

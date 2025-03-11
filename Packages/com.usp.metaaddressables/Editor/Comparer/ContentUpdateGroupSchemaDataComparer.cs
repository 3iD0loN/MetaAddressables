namespace USP.MetaAddressables
{
    public class ContentUpdateGroupSchemaDataComparer : PropertyComparer<MetaAddressables.ContentUpdateGroupSchemaData>
    {
        public ContentUpdateGroupSchemaDataComparer() :
            base((x => x.StaticContent, ObjectComparer.Default))
        {
        }
    }
}

namespace USP.MetaAddressables
{
    public partial class ObjectComparer
    {
        public static bool CompareHash(object leftHand, object rightHand)
        {
            if (leftHand == rightHand)
            {
                return true;
            }

            if (rightHand == null || leftHand == null)
            {
                return false;
            }

            return leftHand.GetHashCode() == rightHand.GetHashCode();
        }
    }
}

namespace USP.MetaAddressables
{
    public interface IEqualityComparer
    {
        #region Methods
        bool Equals(object x, object y);

        int GetHashCode(object obj);
        #endregion
    }
}

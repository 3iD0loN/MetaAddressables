namespace USP.MetaAddressables
{
    // Causes CS0114 if there is no new keyword.
    public interface IItemComparer : System.Collections.IEqualityComparer
    {
    }

    public interface IItemComparer<T> : IItemComparer, System.Collections.Generic.IEqualityComparer<T> 
    {
    }
}

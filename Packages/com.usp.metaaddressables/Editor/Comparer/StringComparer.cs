using System;
using System.Collections.Generic;

namespace USP.MetaAddressables
{
    public class StringComparer : GenericComparer<string>
    {
        #region Static Fields
        public static readonly StringComparer InvariantCulture =
            new StringComparer(System.StringComparer.InvariantCulture);

        public static readonly StringComparer InvariantCultureIgnoreCase =
            new StringComparer(System.StringComparer.InvariantCultureIgnoreCase);

        public static readonly StringComparer Ordinal =
            new StringComparer(System.StringComparer.Ordinal);

        public static readonly StringComparer OrdinalIgnoreCase =
            new StringComparer(System.StringComparer.OrdinalIgnoreCase);

        public static readonly StringComparer CurrentCulture =
            new StringComparer(System.StringComparer.CurrentCulture);

        public static readonly StringComparer CurrentCultureIgnoreCase =
            new StringComparer(System.StringComparer.CurrentCultureIgnoreCase);

        #endregion

        #region Methods
        private StringComparer(IEqualityComparer<string> comparer) :
            base(comparer)
        {
        }

        public StringComparer(Func<string, string, bool> equality = null,
            Func<string, int> hash = null) :
            base(equality, hash)
        {
        }
        #endregion
    }
}

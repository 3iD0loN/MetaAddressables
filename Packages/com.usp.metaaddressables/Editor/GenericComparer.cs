using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;
using static USP.MetaAddressables.MetaAddressables;


namespace USP.MetaAddressables
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        public Func<T, T, bool> Equality;

        public GenericComparer(Func<T, T, bool> equality)
        {
            Equality = equality;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        public virtual bool Equals(T lhs, T rhs)
        {
            return Equality != null ? Equality.Invoke(lhs, rhs) : false;
        }
    }
}

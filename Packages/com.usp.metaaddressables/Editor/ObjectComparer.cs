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
    public static class ObjectComparer
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

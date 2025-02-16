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
    public static class StringComparer
    {
        public static bool CompareOrdinal(string leftHand, string rightHand)
        {
            var lhs = (object)leftHand;
            var rhs = (object)rightHand;

            if (lhs == rhs)
            {
                return true;
            }

            if (rhs == null || lhs == null)
            {
                return false;
            }

            return string.Compare(leftHand, rightHand, StringComparison.Ordinal) == 0;
        }
    }
}

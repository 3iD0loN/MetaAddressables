using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;

using USP.MetaFileExtension;

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
        public abstract class GroupSchemaData : IEqualityComparer<GroupSchemaData>
        {
            #region Static Methods
            public static bool operator ==(GroupSchemaData leftHand, GroupSchemaData rightHand)
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

                return leftHand.GetHashCode() == rightHand.GetHashCode();
            }

            public static bool operator !=(GroupSchemaData lhs, GroupSchemaData rhs)
            {
                return !(lhs == rhs);
            }
            #endregion

            #region Methods
            public abstract override int GetHashCode();

            public override bool Equals(object other)
            {
                if (other is not GroupSchemaData schemaData)
                {
                    return false;
                }

                return this == schemaData;
            }

            public int GetHashCode(GroupSchemaData obj)
            {
                return obj.GetHashCode();
            }

            public bool Equals(GroupSchemaData lhs, GroupSchemaData rhs)
            {
                return lhs == rhs;
            }
            #endregion
        }
        #endregion
    }
}

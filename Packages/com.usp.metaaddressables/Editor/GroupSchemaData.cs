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
        [Serializable]
        public abstract class GroupSchemaData : IEqualityComparer<GroupSchemaData>
        {
            #region Static Methods
            #region Create
            public static AddressableAssetGroupSchema Create(GroupSchemaData groupSchemaData)
            {
                if (groupSchemaData is BundledAssetGroupSchemaData bundledAssetGroupScema)
                {
                    return BundledAssetGroupSchemaData.Create(bundledAssetGroupScema);
                }
                else if (groupSchemaData is ContentUpdateGroupSchemaData contentUpdateGroupSchema)
                {
                    return ContentUpdateGroupSchemaData.Create(contentUpdateGroupSchema);
                }

                return default;
            }

            public static GroupSchemaData Create(AddressableAssetGroupSchema groupSchema)
            {
                if (groupSchema is BundledAssetGroupSchema bundledAssetGroupScema)
                {
                    return new BundledAssetGroupSchemaData(bundledAssetGroupScema);
                }
                else if (groupSchema is ContentUpdateGroupSchema contentUpdateGroupSchemaData)
                {
                    return new ContentUpdateGroupSchemaData(contentUpdateGroupSchemaData);
                }

                return null;
            }

            public static Dictionary<Type, GroupSchemaData> Create(List<AddressableAssetGroupSchema> groupSchemas)
            {
                var result = new Dictionary<Type, GroupSchemaData>(groupSchemas.Count);

                foreach (AddressableAssetGroupSchema groupSchema in groupSchemas)
                {
                    var data = Create(groupSchema);

                    result.Add(data.GetType(), data);
                }

                return result;
            }

            public static List<AddressableAssetGroupSchema> Create(Dictionary<Type, GroupSchemaData> groupSchemaData)
            {
                var result = new List<AddressableAssetGroupSchema>(groupSchemaData.Count);

                foreach (GroupSchemaData schemaData in groupSchemaData.Values)
                {
                    AddressableAssetGroupSchema schema = Create(schemaData);

                    result.Add(schema);
                }

                return result;
            }
            #endregion

            public static bool operator ==(GroupSchemaData leftHand, GroupSchemaData rightHand)
            {
                return ObjectComparer.CompareHash(leftHand, rightHand.GetHashCode());
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

                return Equals(this, schemaData);
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

using System;
using System.Collections.Generic;

using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

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

            public bool Equals(GroupSchemaData leftHand, GroupSchemaData rightHand)
            {
                if (leftHand == rightHand)
                {
                    return true;
                }

                if (rightHand == null || leftHand == null)
                {
                    return false;
                }

                return ObjectComparer.CompareHash(leftHand, rightHand.GetHashCode());
            }
            #endregion
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor.AddressableAssets.Settings;

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
        [Serializable]
        public class GroupData : IEqualityComparer<GroupData>, ISerializationCallbackReceiver
        {
            #region Types
            public static class Comparer
            {
                #region Static Methods
                public static bool CompareName(GroupData leftHand, GroupData rightHand)
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

                    return System.StringComparer.Ordinal.Equals(leftHand.Name, rightHand.Name);
                }

                public static bool CompareNameAndHash(GroupData leftHand, GroupData rightHand)
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

                    return System.StringComparer.Ordinal.Equals(leftHand.Name, rightHand.Name) &&
                        ObjectComparer.CompareHash(leftHand, rightHand);
                }

                public static bool CompareGuid(GroupData leftHand, GroupData rightHand)
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

                    return System.StringComparer.Ordinal.Equals(leftHand.Guid, rightHand.Guid);
                }
                #endregion

                #region Static Fields
                public static readonly GenericComparer<GroupData> ByName = new GenericComparer<GroupData>(CompareName);

                public static readonly GenericComparer<GroupData> ByHash = new GenericComparer<GroupData>(ObjectComparer.CompareHash);

                public static readonly GenericComparer<GroupData> ByNameAndHash = new GenericComparer<GroupData>(CompareNameAndHash);

                public static readonly GenericComparer<GroupData> ByGuid = new GenericComparer<GroupData>(CompareGuid);
                #endregion
            }
            #endregion

            #region Static Methods
            #region Create
            public static AddressableAssetGroup Create(AddressableAssetSettings settings, GroupData groupData)
            {
                List<AddressableAssetGroupSchema> groupSchemas = GroupSchemaData.Create(groupData.SchemaData);

                return settings.CreateGroup(groupData.Name, false, groupData.IsReadOnly, true, groupSchemas);
            }

            public static AddressableAssetGroup Create(AddressableAssetSettings settings, AddressableAssetGroupTemplate groupTemplate)
            {
                return settings.CreateGroup(groupTemplate.Name, false, false, true, groupTemplate.SchemaObjects);
            }
            #endregion

            #region Equality operators
            public static bool operator ==(GroupData leftHand, GroupData rightHand)
            {
                return ObjectComparer.CompareHash(leftHand, rightHand);
            }

            public static bool operator !=(GroupData lhs, GroupData rhs)
            {
                return !(lhs == rhs);
            }
            #endregion
            #endregion

            #region Fields
            [SerializeField]
            private string _guid;

            [SerializeField]
            private bool _readOnly;

            [SerializeReference]
            private GroupSchemaData[] _schemaData;
            #endregion

            #region Properties
            public string Name { get; }

            public string Guid => _guid;

            public bool IsReadOnly => _readOnly;

            public Dictionary<Type, GroupSchemaData> SchemaData
            {
                get;
                private set;
            }
            #endregion

            #region Methods
            #region Constructors
            public GroupData(AddressableAssetGroupTemplate groupTemplate) :
                this(groupTemplate.Name, string.Empty, false, GroupSchemaData.Create(groupTemplate.SchemaObjects))
            {
            }

            public GroupData(AddressableAssetGroup group) :
                this(group.Name, group.Guid, group.ReadOnly, GroupSchemaData.Create(group.Schemas))
            {
            }

            private GroupData(string name, string guid, bool readOnly, Dictionary<Type, GroupSchemaData> groupSchemaData)
            {
                Name = name;

                _guid = guid;

                _readOnly = readOnly;

                _schemaData = new GroupSchemaData[0];
                SchemaData = groupSchemaData ?? new Dictionary<Type, GroupSchemaData>();
            }
            #endregion

            #region ISerializationCallbackReceiver
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                _schemaData = new GroupSchemaData[SchemaData.Count];
                SchemaData.Values.CopyTo(_schemaData, 0);
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                var result = new Dictionary<Type, GroupSchemaData>(_schemaData.Length);
                foreach (GroupSchemaData groupSchema in _schemaData)
                {
                    result.Add(groupSchema.GetType(), groupSchema);
                }

                SchemaData = result;
            }
            #endregion

            public override int GetHashCode()
            {
                int result = 17;

                result = result * 31 ^ _readOnly.GetHashCode();

                int result2 = 17;
                foreach (var pair in SchemaData)
                {
                    int result3 = 17;
                    result3 = result3 * 31 ^ pair.Key.GetHashCode();
                    result3 = result3 * 31 ^ pair.Value.GetHashCode();

                    result2 = result2 * 31 ^ result3;
                }

                result = result * 31 ^ result2;

                return result;
            }

            public override bool Equals(object other)
            {
                if (other is not GroupData group)
                {
                    return false;
                }

                return Equals(this, group);
            }

            public int GetHashCode(GroupData obj)
            {
                return obj.GetHashCode();
            }

            public bool Equals(GroupData lhs, GroupData rhs)
            {
                return lhs == rhs;
            }

            public override string ToString()
            {
                return Name;
            }

            public T GetSchemaData<T>() 
                where T : GroupSchemaData
            {
                var type = typeof(T);
                bool found = SchemaData.TryGetValue(type, out GroupSchemaData value);

                if (found && value is T schemaData)
                {
                    return schemaData;
                }

                return default;
            }
            #endregion
        }
        #endregion
    }
}

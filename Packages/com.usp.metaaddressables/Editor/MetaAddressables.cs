using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using USP.MetaFileExtension;

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Constants
        private const string UserDataKey = "MetaAddressables";
        #endregion

        #region Static Fields
        public static readonly Factory factory;
        #endregion

        #region Static Methods
        static MetaAddressables()
        {
            factory = new Factory();
        }

        public static UserData Read(string assetFilePath)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetFilePath);

            if (assetImporter == null)
            {
                return null;
            }

            return MetaFile.Read(assetImporter, UserDataKey, factory.Create);
        }

        public static (AddressableAssetGroup, GroupData) Generate(GroupData groupData, AddressableAssetSettings settings)
        {
            AddressableAssetGroup group;

            // If there is no group GUID assigned to the userdata,
            // then either we haven't found a prexisting group that matches,
            // or the group data represents a template and there is no prexisting group that matches.
            if (!string.IsNullOrEmpty(groupData.Guid))
            {
                // Attempt to find a group associated with the guid.
                // If there was a group found that was associated with the guid, then:
                if (AddressablesLookup.GroupsAndHashesByGuids.TryGetValue(groupData.Guid, out group))
                {
                    // Return the found group and no group data since there is no modifications made to it.
                    // Do nothing else.
                    return (group, null);
                }
            }

            // Otherwise, there was no group found associated with the guid, and/or no guid exists.

            // Attempt to find an existing group with the identical properties, or create a one from the group data.

            // Get the hash code for the group (which hashed the values of the properties).
            int hash = groupData.GetHashCode();

            // Attempt to find a list of groups that are associated with the group data.
            bool found = AddressablesLookup.GroupsByPropertyHash.TryGetValue(hash,
                out List<AddressableAssetGroup> groupList);

            // If a list of groups is associated with the hash, then:
            if (found)
            {
                // Get the first group in the list.
                group = groupList.First();

                // Create a new group data instance based off of the group.
                groupData = new GroupData(group);

                return (group, groupData);
            }

            // Otherwise, there is no group associated with the hash.

            // Add the group to the Addressables system (and AddressablesLookup)
            group = GroupData.Create(settings, groupData);

            // Return the found group and no group data since there is no modifications made to it.
            return (group, null);
        }

        public static AddressableAssetEntry Generate(ref UserData userData, AddressableAssetSettings settings)
        {
            if (settings == null)
            {
                return null;
            }

            (AddressableAssetGroup group, GroupData groupData) = Generate(userData.Group, settings);
            if (groupData != null)
            {
                userData.Group = groupData;
            }

            return AssetData.CreateOrMove(settings, group, userData.Asset);
        }

        public static void Clear(string assetFilePath)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetFilePath);

            MetaFile.Clear(assetImporter, UserDataKey);
        }

        public static void Write(string assetFilePath, UserData userData)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetFilePath);

            MetaFile.Write(assetImporter, UserDataKey, userData);
        }
        #endregion
    }
}

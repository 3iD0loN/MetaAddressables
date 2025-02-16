using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using USP.MetaFileExtension;
using static USP.MetaAddressables.MetaAddressables;

namespace USP.MetaAddressables
{
    public static class AddressablesGroupLookup
    {
        #region Static Properties
        public static Dictionary<string, AddressableAssetGroup> GroupsByGuids { get; }

        public static Dictionary<int, List<AddressableAssetGroup>> GroupsByHash { get; }
        #endregion

        #region Static Methods
        static AddressablesGroupLookup()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                // NOTE: should throw an error?

                return;
            }

            GroupsByGuids = new Dictionary<string, AddressableAssetGroup>(settings.groups.Count);
            GroupsByHash = new Dictionary<int, List<AddressableAssetGroup>>(settings.groups.Count);

            foreach (var group in settings.groups)
            {
                if (group == null)
                {
                    continue;
                }

                GroupsByGuids.Add(group.Guid, group);

                // Pack the group into a group data...
                var groupData = new GroupData(group);

                // so that we can generate a hash that is unique to its properties.
                int hash = groupData.GetHashCode();

                // Attempt to get the list of Addressable groups that are associated with the hash.
                // (More than one group might have the same property values, so their property hashes might collide).
                bool found = GroupsByHash.TryGetValue(hash, out List<AddressableAssetGroup> groupList);

                // If there is no list of groups associated with the hash, then: 
                if (!found)
                {
                    // Create a new list of groups.
                    groupList = new List<AddressableAssetGroup>();

                    // Associate the new list of groups with the property hash.
                    GroupsByHash.Add(hash, groupList);
                }

                // There exists a list of groups associated with the property hash.

                // Add the group to the list.
                groupList.Add(group);
            }
        }
        #endregion
    }

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

        private static AddressableAssetGroup Generate(AddressableAssetSettings settings,
            ref GroupData groupData)
        {
            AddressableAssetGroup group;

            // If there is no group GUID assigned to the userdata,
            // then either we haven't found a prexisting group that matches,
            // or there is no prexisting one that matches.
            if (!string.IsNullOrEmpty(groupData.Guid))
            {
                // Attempt to find a group associated with the guid.
                // If there was a group found that was associated with the guid, then:
                if (AddressablesGroupLookup.GroupsByGuids.TryGetValue(groupData.Guid, out group))
                {
                    // Return the found group. Do nothing else.
                    return group;
                }

                // Otherwise, there was no group found associated with the guid.

                // Attempt to find another existing group or create a new group.
            }

            // Get the hash code for the group.
            int hash = groupData.GetHashCode();

            // Attempt to find a list of groups that are associated with the group data.
            bool found = AddressablesGroupLookup.GroupsByHash.TryGetValue(hash,
                out List<AddressableAssetGroup> groupList);

            // If a list of groups is associated with the hash, then:
            if (found)
            {
                // Get the first group in the list.
                group = groupList.First();

                // Create a new group data instance based off of the group.
                groupData = new GroupData(group);

                return group;
            }

            // Otherwise, there is no group associated with the hash.

            // Create a new list of groups
            groupList = new List<AddressableAssetGroup>();

            // Associate the list with the hash. 
            AddressablesGroupLookup.GroupsByHash.Add(hash, groupList);

            group = GroupData.Create(settings, groupData);

            groupList.Add(group);

            return group;
        }

        public static void Generate(UserData userData)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var groupData = userData.Group;
            AddressableAssetGroup group = Generate(settings, ref groupData);
            userData.Group = groupData;

            AssetData.Create(settings, group, userData.Asset);
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

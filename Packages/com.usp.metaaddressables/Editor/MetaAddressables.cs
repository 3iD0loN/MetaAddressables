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
                // If there was a group found that was associated with the guid, thenG
                if (AddressablesLookup.GroupsAndHashesByGuids.TryGetValue(groupData.Guid, out group))
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
            bool found = AddressablesLookup.GroupsByPropertyHash.TryGetValue(hash,
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
            AddressablesLookup.GroupsByPropertyHash.Add(hash, groupList);

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

            AssetData.CreateOrMove(settings, group, userData.Asset);
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

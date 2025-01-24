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
using static USP.MetaAddressables.MetaAddressables;

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Constants
        private const string UserDataKey = "MetaAddressables";
        #endregion

        #region Static Fields
        public static readonly Factory factory;

        private static Dictionary<string, AddressableAssetGroup> s_groupsByGuids;

        private static Dictionary<int, List<AddressableAssetGroup>> s_groupsByHash;
        #endregion

        #region Static Methods
        static MetaAddressables()
        {
            factory = new Factory();

            s_groupsByGuids = new Dictionary<string, AddressableAssetGroup>();

            s_groupsByHash = new Dictionary<int, List<AddressableAssetGroup>>();

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            foreach (var group in settings.groups)
            {
                if (group == null)
                {
                    continue;
                }

                s_groupsByGuids.Add(group.Guid, group);

                var groupData = new GroupData(group);
                int hash = groupData.GetHashCode();

                bool found = s_groupsByHash.TryGetValue(hash, out List<AddressableAssetGroup> groupList);

                if (!found)
                {
                    groupList = new List<AddressableAssetGroup>();
                    s_groupsByHash.Add(hash, groupList);
                }

                groupList.Add(group);
            }
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

        public static AddressableAssetGroup Generate(ref GroupData groupData)
        {
            AddressableAssetGroup group;

            // If there is no group GUID assigned to the userdata,
            // then either we haven't found a prexisting one that matches,
            // or there is no prexisting one that matches.
            if (!string.IsNullOrEmpty(groupData.Guid))
            {
                // Attempt to find a group associated with the guid.
                // If there was a group found that was associated with the guid, then:
                if (s_groupsByGuids.TryGetValue(groupData.Guid, out group))
                {
                    // Return the found group.
                    return group;
                }

                // Otherwise, there was no group found associated with the guid.

                // Attempt to find another existing group or create a new group.
            }

            // Get the hash code for the group.
            int hash = groupData.GetHashCode();

            bool found = s_groupsByHash.TryGetValue(hash,
                out List<AddressableAssetGroup> groupList);

            // If a list of groups is associated with the hash, then:
            if (found)
            {
                group = groupList.First();

                // Create new group data based off of this group.
                groupData = new GroupData(group);

                return group;
            }

            // Otherwise, there is no group associated with the hash.

            // Create a new list of groups
            groupList = new List<AddressableAssetGroup>();

            // Associate the list with the hash. 
            s_groupsByHash.Add(hash, groupList);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            List<AddressableAssetGroupSchema> schemas = groupData.CreateSchemas();
            group = settings.CreateGroup("New Group", false, groupData.ReadOnly, false, schemas);

            groupList.Add(group);

            return group;
        }

        public static AddressableAssetEntry Generate(AddressableAssetGroup group, AssetData assetData)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetData.Guid, group);
            entry.SetAddress(assetData.Address);

            foreach (var label in assetData.Labels)
            {
                entry.labels.Add(label);
            }

            return entry;
        }

        public static void Generate(UserData userData)
        {
            var groupData = userData.Group;
            AddressableAssetGroup group = Generate(ref groupData);
            userData.Group = groupData;

            Generate(group, userData.Asset);
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

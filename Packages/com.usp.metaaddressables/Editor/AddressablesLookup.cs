using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using UnityEngine;

using USP.MetaFileExtension;

namespace USP.MetaAddressables
{
    [InitializeOnLoad]
    public static class AddressablesLookup
    {
        #region Static Properties
        public static Dictionary<string, (int, AddressableAssetGroup)> GroupsAndHashesByGuids { get; }

        public static Dictionary<int, List<AddressableAssetGroup>> GroupsByPropertyHash { get; }
        #endregion

        #region Static Methods
        static AddressablesLookup()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            
            if (settings == null)
            {
                Debug.LogWarning("Could not find default Addressables Settings object. Was it created?");

                return;
            }

            settings.OnModification -= OnSettingsModification;
            settings.OnModification += OnSettingsModification;

            GroupsAndHashesByGuids = new Dictionary<string, (int, AddressableAssetGroup)>(settings.groups.Count);
            GroupsByPropertyHash = new Dictionary<int, List<AddressableAssetGroup>>(settings.groups.Count);

            foreach (AddressableAssetGroup group in settings.groups)
            {
                AddGroup(group);
            }
        }

        private static void AddGroup(AddressableAssetGroup group)
        {
            if (group == null)
            {
                return;
            }

            // Pack the group into a group data...
            var groupData = new MetaAddressables.GroupData(group);

            // so that we can generate a hash that is unique to its properties.
            int hash = groupData.GetHashCode();

            GroupsAndHashesByGuids.Add(group.Guid, (hash, group));

            // Attempt to get the list of Addressable groups that are associated with the hash.
            // (More than one group might have the same property values, so their property hashes might collide).
            bool found = GroupsByPropertyHash.TryGetValue(hash, out List<AddressableAssetGroup> groupList);

            // If there is no list of groups associated with the hash, then: 
            if (!found)
            {
                // Create a new list of groups.
                groupList = new List<AddressableAssetGroup>();

                // Associate the new list of groups with the property hash.
                GroupsByPropertyHash.Add(hash, groupList);
            }

            // There exists a list of groups associated with the property hash.

            // Add the group to the list.
            groupList.Add(group);
        }

        private static void RemoveGroup(AddressableAssetGroup group)
        {
            if (group == null)
            {
                return;
            }

            bool found = GroupsAndHashesByGuids.TryGetValue(group.Guid, out (int Hash, AddressableAssetGroup) value);

            if (!found)
            {
                return;
            }

            GroupsAndHashesByGuids.Remove(group.Guid);

            // Attempt to get the list of Addressable groups that are associated with the hash.
            // (More than one group might have the same property values, so their property hashes might collide).
            found = GroupsByPropertyHash.TryGetValue(value.Hash, out List<AddressableAssetGroup> groupList);

            // If there is no list of groups associated with the hash, then: 
            if (!found)
            {
                return;
            }

            // Create a new list of groups.
            groupList.Remove(group);

            if (groupList.Count == 0)
            {
                GroupsByPropertyHash.Remove(value.Hash);
            }
        }

        private static void OnSettingsModification(AddressableAssetSettings settings,
                AddressableAssetSettings.ModificationEvent modificationEvent, object eventData)
        {
            switch (modificationEvent)
            {
                case AddressableAssetSettings.ModificationEvent.GroupAdded:

                    if (eventData is AddressableAssetGroup addedGroup)
                    {
                        AddGroup(addedGroup);
                    }

                    break;
                case AddressableAssetSettings.ModificationEvent.GroupRemoved:

                    if (eventData is AddressableAssetGroup removedGroup)
                    {
                        RemoveGroup(removedGroup);
                    }

                    break;
            }
        }

        #endregion
    }
}

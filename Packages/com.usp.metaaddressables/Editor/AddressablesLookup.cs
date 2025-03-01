using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using USP.MetaFileExtension;

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
                var groupData = new MetaAddressables.GroupData(group);

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
}

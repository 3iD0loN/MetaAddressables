/*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
////using UnityEngine.TestTools;
///
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

////[InitializeOnLoad]
static class MetaAddressablesAssetInspector
{
    static MetaAddressablesAssetInspector()
    {
        ////Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
    }

    static void OnPostHeaderGUI(Editor editor)
    {
        if (editor.targets.Length <= 0)
        {
            return;
        }

        // Determine whether the editor target is a  Prefab/Model importer not the displayed GameObjects
        var isGameObject = editor.target.GetType() == typeof(GameObject);

        if (isGameObject)
        {
            return;
        }

        var backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;

        ////X.DrawAddressablesGroupIncludeInBuildToggle(editor.targets);

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

        ///Y.DrawAddressablesGroupSchemaProfile(settings, editor.targets);

        Z.Draw(settings, editor);

        // Set the background color to the previous state.
        GUI.backgroundColor = backgroundColor;
    }
}

static class X
{
    private static readonly GUIContent s_includeInBuildGUIContent = new GUIContent("Include in Build", "Determines if assets in this group will be included or excluded in the Addressables build.");

    private static GUIStyle s_toggleMixed;

    public static void DrawAddressablesGroupIncludeInBuildToggle(UnityEngine.Object[] targets)
    {
        var schemas = new List<BundledAssetGroupSchema>();

        foreach (UnityEngine.Object target in targets)
        {
            if (!(target is AddressableAssetGroup group))
            {
                break;
            }

            var schema = group.GetSchema<BundledAssetGroupSchema>();

            if (schema != null)
            {
                schemas.Add(schema);
            }
        }

        if (schemas.Count != targets.Length)
        {
            return;
        }

        bool sameValue = true;
        for (int i = 0; i < schemas.Count; ++i)
        {
            sameValue = schemas[0].IncludeInBuild == schemas[i].IncludeInBuild;
        }

        EditorGUI.BeginChangeCheck();

        bool newIncludeInBuildValue;

        if (schemas.Count > 1 && !sameValue)
        {
            if (s_toggleMixed == null)
            {
                s_toggleMixed = new GUIStyle("ToggleMixed");
            }

            newIncludeInBuildValue = GUILayout.Toggle(false, s_includeInBuildGUIContent, s_toggleMixed);
        }
        else
        {
            newIncludeInBuildValue = GUILayout.Toggle(schemas[0].IncludeInBuild, s_includeInBuildGUIContent);
        }

        if (EditorGUI.EndChangeCheck())
        {
            foreach (BundledAssetGroupSchema schema in schemas)
            {
                schema.IncludeInBuild = newIncludeInBuildValue;
            }
        }
    }
}

static class Y
{
    private static readonly GUIContent s_systemSettingsGUIContent = new GUIContent("System Settings", "View Addressable Asset Settings");

    public static void DrawAddressablesGroupSchemaProfile(AddressableAssetSettings settings, UnityEngine.Object[] targets)
    {
        foreach (var target in targets)
        {
            if (target is AddressableAssetGroupSchema)
            {
                GUILayout.BeginHorizontal();

                var profileName = settings.profileSettings.GetProfileName(settings.activeProfileId);

                GUILayout.Label("Profile: " + profileName);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(s_systemSettingsGUIContent, "MiniButton"))
                {
                    EditorGUIUtility.PingObject(settings);
                    Selection.activeObject = settings;
                }

                GUILayout.EndHorizontal();

                return;
            }
        }
    }
}

static class Z
{
    #region Types
    internal class Cache<T1, T2>
    {
        #region Fields
        private AddressableAssetSettings _settings;
        
        private Dictionary<T1, T2> _targetInfoCache;

        private Hash128 _currentCacheVersion;
        #endregion

        #region Methods
        public Cache(AddressableAssetSettings settings)
        {
            _settings = settings;
            _targetInfoCache = new Dictionary<T1, T2>();

        }

        /// <summary>
        /// Determine if the cache is in a valid state.
        /// </summary>
        /// <returns>A value indicating whether the cache is a valid state.</returns>
        private bool IsValid()
        {
            // If there are no target infos in the cache, then:
            if (_targetInfoCache.Count == 0)
            {
                // The cache is not in a valid state.
                return false;
            }

            // Otherwise, there are target infos in the cache.

            // If the cache version hash valid and it matches the settings hash, then:
            if (_currentCacheVersion.isValid &&
                _currentCacheVersion.Equals(_settings.currentHash))
            {
                // The cache is in a valid state.
                return true;
            }

            // Otherwise, the hash is either invalid or it does not match the settings hash.

            // Clear the collection of items.
            _targetInfoCache.Clear();

            // Clear the hash.
            _currentCacheVersion = default;

            // The cache is not in a valid state.
            return false;
        }

        public void Add(T1 key, T2 value)
        {
            // If the cache is not valid, then:
            if (!IsValid())
            {
                // Update the hash with the hash from the settings.
                _currentCacheVersion = _settings.currentHash;
            }

            // Add the key and value to the cache.
            _targetInfoCache.Add(key, value);
        }

        public void Remove(T1 key)
        {
            // If the cache is not valid, then:
            if (!IsValid())
            {
                // Update the hash with the hash from the settings.
                _currentCacheVersion = _settings.currentHash;
            }

            // Remove the key and value from the cache.
            _targetInfoCache.Remove(key);
        }

        public bool TryGetCached(T1 key, out T2 result)
        {
            // If the cache is valid and there is a value associated with the key, then:
            if (IsValid() && _targetInfoCache.TryGetValue(key, out result))
            {
                // An item was successfuly found.
                return true;
            }

            // Otherwise, either the cache is not valid or there is no value associated with the key, then:

            // The value is defaulted.
            result = default;

            // There was no item successfully found.
            return false;
        }
        #endregion
    }

    internal class TargetInfo
    {
        public UnityEngine.Object TargetObject;
        public string Guid;
        public string Path;
        public bool IsMainAsset;
        public AddressableAssetEntry MainAssetEntry;

        public string Address
        {
            get
            {
                if (MainAssetEntry == null)
                {
                    throw new NullReferenceException("No Entry set for Target info with AssetPath " + Path);
                }

                return MainAssetEntry.address;
            }
        }
    }

    /// <summary>
    /// Static class of common strings and string formats used through out the build process
    /// </summary>
    public static class CommonStrings
    {
        /// <summary>
        /// Unity Editor Resources path
        /// </summary>
        public const string UnityEditorResourcePath = "library/unity editor resources";

        internal const string UnityEditorResourceGuid = "0000000000000000d000000000000000";

        /// <summary>
        /// Unity Default Resources path
        /// </summary>
        public const string UnityDefaultResourcePath = "library/unity default resources";

        internal const string UnityDefaultResourceGuid = "0000000000000000e000000000000000";

        /// <summary>
        /// Unity Built-In Extras path
        /// </summary>
        public const string UnityBuiltInExtraPath = "resources/unity_builtin_extra";

        internal const string UnityBuiltInExtraGuid = "0000000000000000f000000000000000";

        /// <summary>
        /// Default Asset Bundle internal file name format
        /// </summary>
        public const string AssetBundleNameFormat = "archive:/{0}/{0}";

        /// <summary>
        /// Default Scene Bundle internal file name format
        /// </summary>
        public const string SceneBundleNameFormat = "archive:/{0}/{1}.sharedAssets";
    }
    #endregion

    #region Static Fields
    private static readonly GUIContent s_AddressableAssetToggleText = new GUIContent("Addressable",
        "Check this to mark this asset as an Addressable Asset, which includes it in the bundled data and makes it loadable via script by its address.");

    private static string isEditorFolder = $"{Path.DirectorySeparatorChar}Editor";
    
    private static string insideEditorFolder = $"{Path.DirectorySeparatorChar}Editor{Path.DirectorySeparatorChar}";

    private static HashSet<string> excludedExtensions = new HashSet<string>(new string[] { ".cs", ".js", ".boo", ".exe", ".dll", ".meta", ".preset", ".asmdef" });

    private static GUIStyle s_toggleMixed;

    static GUIContent s_GroupsDropdownLabelContent = new GUIContent("Group", "The Addressable Group that this asset is assigned to.");

    static int s_MaxLabelCharCount = 35;
    static GUIContent s_ConfigureLabelsGUIContent = new GUIContent("", "Configure Addressables Labels");
    static int s_RemoveButtonWidth = 8;
    static GUIContent s_RemoveButtonGUIContent = new GUIContent("", EditorGUIUtility.IconContent("toolbarsearchCancelButtonActive").image);

    static GUIStyle s_AssetLabelStyle = null;
    static GUIStyle s_AssetLabelIconStyle = null;
    static GUIStyle s_AssetLabelXButtonStyle = null;
    #endregion

    #region Fields
    // Caching due to Gathering TargetInfos is an expensive operation
    // The InspectorGUI needs to call this multiple times per layout and paint
    private static Cache<int, List<TargetInfo>> s_cache = null;
    #endregion

    #region Static Methods
    private static bool IsPathValidPackageAsset(string pathLowerCase)
    {
        string[] splitPath = pathLowerCase.Split(Path.DirectorySeparatorChar);

        if (splitPath.Length < 3)
        {
            return false;
        }

        if (!String.Equals(splitPath[0], "packages", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (String.Equals(splitPath[2], "package.json", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private static bool StringContains(string input, string value, StringComparison comp)
    {
#if NET_UNITY_4_8
        return input.Contains(value, comp);
#else
        return input.Contains(value);
#endif
    }

    private static bool IsPathValidForEntry(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (path.Contains('\\'))
            path = path.Replace('\\', Path.DirectorySeparatorChar);

        if (Path.DirectorySeparatorChar != '/' && path.Contains('/'))
            path = path.Replace('/', Path.DirectorySeparatorChar);

        if (!path.StartsWith("Assets", StringComparison.OrdinalIgnoreCase) && !IsPathValidPackageAsset(path))
            return false;

        string ext = Path.GetExtension(path);
        if (string.IsNullOrEmpty(ext))
        {
            // is folder
            if (path == "Assets")
                return false;

            int editorIndex = path.IndexOf(isEditorFolder, StringComparison.OrdinalIgnoreCase);
            if (editorIndex != -1)
            {
                int length = path.Length;
                if (editorIndex == length - 7)
                {
                    return false;
                }

                if (path[editorIndex + 7] == '/')
                {
                    return false;
                }

                // Could still have something like Assets/editorthings/Editor/things, but less likely
                if (StringContains(path, insideEditorFolder, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            if (String.Equals(path, CommonStrings.UnityEditorResourcePath, StringComparison.Ordinal) ||
                String.Equals(path, CommonStrings.UnityDefaultResourcePath, StringComparison.Ordinal) ||
                String.Equals(path, CommonStrings.UnityBuiltInExtraPath, StringComparison.Ordinal))
            {
                return false;
            }
        }
        else
        {
            // asset type
            if (StringContains(path, insideEditorFolder, StringComparison.OrdinalIgnoreCase))
                return false;

            if (excludedExtensions.Contains(ext))
                return false;
        }

        var settings = AddressableAssetSettingsDefaultObject.SettingsExists ? AddressableAssetSettingsDefaultObject.Settings : null;
        if (settings != null && path.StartsWith(settings.ConfigFolder, StringComparison.Ordinal))
            return false;

        return true;
    }

    private static bool TryGetPathAndGUIDFromTarget(UnityEngine.Object target, out string path, out string guid)
    {
        // If the target is invalid, then:
        if (target == null)
        {
            // The asset GUID and the asset file path are not valid.
            guid = string.Empty;
            path = string.Empty;

            // The asset GUID and asset file path was not sucessfully found.
            return false;
        }

        // Otherwise, the target is valid.

        // Attempt to get the asset GUID and local ID associated with the target.
        // If there are no asset GUID or local ID is associated with the target, then:
        if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(target, out guid, out long id))
        {
            // The asset GUID and the asset file path are not valid.
            guid = string.Empty;
            path = string.Empty;

            // The asset GUID and asset file path was not sucessfully found.
            return false;
        }

        // Otherwise, there is an asset GUID or local ID associated with the target.

        // Get the asset file path associated with the target.
        path = AssetDatabase.GetAssetOrScenePath(target);

        // Determine whether the asset file path is valid.
        // If the asset file path is not valid, then:
        if (!IsPathValidForEntry(path))
        {
            // The asset GUID and asset file path was not sucessfully found.
            return false;
        }

        // Otherwise, the asset file path is valid.

        // The asset GUID and asset file path were sucessfully found.
        return true;
    }

    private static int BlochHash(UnityEngine.Object[] targets)
    {
        // Get the first target's hash code.
        int selectionHashCode = targets[0].GetHashCode();

        // For every target after the first target , perform the following:
        for (int i = 1; i < targets.Length; ++i)
        {
            // Create a hash code.
            selectionHashCode = selectionHashCode * 31 ^ targets[i].GetHashCode();
        }

        return selectionHashCode;
    }

    private static List<TargetInfo> GatherTargetInfos(AddressableAssetSettings settings, UnityEngine.Object[] targets)
    {
        // If a cache does not exist and there are valid settings, then:
        if (s_cache == null && settings != null)
        {
            // Create a new instance of the cache based off of the settings.
            s_cache = new Cache<int, List<TargetInfo>>(settings);
        }

        // Get the first target's hash code.
        int selectionHashCode = BlochHash(targets);

        List<TargetInfo> targetInfos = null;

        // If there is a valid cache,
        // and there is a successful attempt to retrieve target infos associated with the target hash, then:
        if (s_cache != null && s_cache.TryGetCached(selectionHashCode, out targetInfos))
        {
            // Assume that all target infos are valid.
            bool isValid = true;

            // For every target info in the list, perform the following:
            foreach (var targetInfo in targetInfos)
            {
                // If there is a target info with an invalid parent group, then:
                if (targetInfo.MainAssetEntry?.parentGroup == null)
                {
                    // There is at least one target info in the group that is invalid. 
                    isValid = false;

                    // Do not iterate through the rest.
                    break;
                }
            }

            // If all the target infos are valid, then:
            if (isValid)
            {
                // Return the target info.
                return targetInfos;
            }

            // Otherwise, not all the target infos are valid.

            // Remove the entry and target infos.
            s_cache.Remove(selectionHashCode);
        }

        // There is no target info associated with the targets.

        // Create a new list of target infos.
        targetInfos = new List<TargetInfo>(targets.Length);

        AddressableAssetEntry entry;
        foreach (var target in targets)
        {
            // Attempt to get the asset path and asset guid from the instance
            if (TryGetPathAndGUIDFromTarget(target, out var path, out var guid))
            {
                // Get the type of the main asset at the path.
                var mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);

                // If the asset type is valid and the assembly that the type is defined in is a runtime assembly, then:
                if (mainAssetType != null && !BuildUtility.IsEditorAssembly(mainAssetType.Assembly))
                {
                    // If the target is an asset importer or the target is the main asset.
                    bool isMainAsset = target is AssetImporter || AssetDatabase.IsMainAsset(target);

                    // Create the info about the target.
                    var info = new TargetInfo()
                    {
                        TargetObject = target,
                        Guid = guid,
                        Path = path,
                        IsMainAsset = isMainAsset
                    };
                    
                    // If the settings are valid, then:
                    if (settings != null)
                    {
                        // Get the Addressable asset entry associated with the guid.
                        entry = settings.FindAssetEntry(guid, true);

                        // If the Addressable asset entry exists, then:
                        if (entry != null)
                        {
                            // Set the main asset entry.
                            info.MainAssetEntry = entry;
                        }
                    }

                    // Add the target info to the list.
                    targetInfos.Add(info);
                }
            }
        }

        // If the cache is valid and the list of target infos are valid,
        // and there are items in the list of target infos, then:
        if (s_cache != null && targetInfos != null && targetInfos.Count > 0)
        {
            // Add the list of target infos to the cache.
            s_cache.Add(selectionHashCode, targetInfos);
        }

        // Return the list of target infos.
        return targetInfos;
    }

    public static void Draw(AddressableAssetSettings settings, Editor editor)
    {
        // Transform the targets to target info types.
        List<Z.TargetInfo> targetInfos = Z.GatherTargetInfos(settings, editor.targets);

        // If there are no valid target infos, then:
        if (targetInfos.Count == 0)
        {
            // Do nothing else.
            return;
        }

        if (s_toggleMixed == null)
        {
            s_toggleMixed = new GUIStyle("ToggleMixed");
        }

        // Otherwise, there are valid target infos.

        bool targetHasAddressableSubObject = false;
        int mainAssetsAddressable = 0;
        int subAssetsAddressable = 0;

        // For every target info in the list, perform the following:
        foreach (TargetInfo info in targetInfos)
        {
            // If there is no main asset associated with the target, then:
            if (info.MainAssetEntry == null)
            {
                // Skip this target info.
                continue;
            }

            // Otherwise, there is a main asset associated with the target.

            // If the asset is a sub-asset, then:
            if (info.MainAssetEntry.IsSubAsset)
            {
                // Increase the number of sub-assets that are Addressable.
                subAssetsAddressable++;
            }
            else
            {
                // Otherwise, the asset is a main asset.

                // Increase the number of main assets that are Addressbake.
                mainAssetsAddressable++;
            }

            // If the target does not represent a main asset, then:
            if (!info.IsMainAsset)
            {
                // The target has a sub asset that is Addressable.
                targetHasAddressableSubObject = true;
            }
        }

        // Cache the enabled state back ro revert it later.
        bool prevEnabledState = GUI.enabled;

        // If the targets has a sub-asset that is Addressable, then:
        if (targetHasAddressableSubObject)
        {
            // Disable the GUI controls.
            GUI.enabled = false;
        }
        else
        {
            // Otherwise, the targets do not have Addressable sub-assets.

            // Enable the GUI controls.
            GUI.enabled = true;

            // For every target info in the list, perform the following:
            foreach (var info in targetInfos)
            {
                // If any of the targets are not a main asset, then:
                if (!info.IsMainAsset)
                {
                    // Disable the GUI controls.
                    GUI.enabled = false;

                    // We found one sub-assets, so do not process any of the other targets.
                    break;
                }
            }
        }

        // Add together the main and sub-assets.
        int totalAddressableCount = mainAssetsAddressable + subAssetsAddressable;
        
        // If there are no Addressable assets in the selected targets, then:
        if (totalAddressableCount == 0)
        {
            if (GUILayout.Toggle(false, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false)))
            {
                ////SetAaEntry(AddressableAssetSettingsDefaultObject.GetSettings(true), targetInfos, true);
            }
        }
        else if (totalAddressableCount == editor.targets.Length) // everything is addressable
        {
            var entryInfo = targetInfos[targetInfos.Count - 1];

            if (entryInfo == null || entryInfo.MainAssetEntry == null)
            {
                throw new NullReferenceException("EntryInfo incorrect for Addressables content.");
            }

            GUILayout.BeginHorizontal();

            if (mainAssetsAddressable > 0 && subAssetsAddressable > 0)
            {
                if (s_toggleMixed == null)
                {
                    s_toggleMixed = new GUIStyle("ToggleMixed");
                }

                if (GUILayout.Toggle(false, s_AddressableAssetToggleText, s_toggleMixed, GUILayout.ExpandWidth(false)))
                {
                    ////SetAaEntry(settings, targetInfos, true);
                }
            }
            else if (mainAssetsAddressable > 0)
            {
                if (!GUILayout.Toggle(true, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false)))
                {
                    ////SetAaEntry(settings, targetInfos, false);
                    
                    GUI.enabled = prevEnabledState;
                    
                    GUIUtility.ExitGUI();
                }
            }
            else if (GUILayout.Toggle(false, s_AddressableAssetToggleText, GUILayout.ExpandWidth(false)))
            {
                ////SetAaEntry(settings, targetInfos, true);
            }

            if (editor.targets.Length == 1)
            {
                if (!entryInfo.IsMainAsset || entryInfo.MainAssetEntry.IsSubAsset)
                {
                    bool preAddressPrevEnabledState = UnityEngine.GUI.enabled;
                    GUI.enabled = false;
                    
                    string address = entryInfo.Address + (entryInfo.IsMainAsset ? "" : $"[{entryInfo.TargetObject.name}]");
                    
                    EditorGUILayout.DelayedTextField(address, GUILayout.ExpandWidth(true));
                    
                    GUI.enabled = preAddressPrevEnabledState;
                }
                else
                {
                    string newAddress = EditorGUILayout.DelayedTextField(entryInfo.Address, GUILayout.ExpandWidth(true));
                    if (newAddress != entryInfo.Address)
                    {
                        if (newAddress.Contains('[') && newAddress.Contains(']'))
                        {
                            Debug.LogErrorFormat("Rename of address '{0}' cannot contain '[ ]'.", entryInfo.Address);
                        }
                        else
                        {
                            entryInfo.MainAssetEntry.address = newAddress;
                            AddressableAssetUtility.OpenAssetIfUsingVCIntegration(entryInfo.MainAssetEntry.parentGroup, true);
                        }
                    }
                }
            }
            else
            {
                FindUniqueAssetGuids(targetInfos, out var uniqueAssetGuids, out var uniqueAddressableAssetGuids);
                EditorGUILayout.LabelField(uniqueAddressableAssetGuids.Count + " out of " + uniqueAssetGuids.Count + " assets are addressable.");
            }

            DrawSelectEntriesButton(targetInfos);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawGroupsDropdown(settings, targetInfos);
            GUILayout.EndHorizontal();

            DrawLabels(targetInfos, settings, editor);
        }
        else // mixed addressable selected
        {
            GUILayout.BeginHorizontal();

            if (s_toggleMixed == null)
            {
                s_toggleMixed = new GUIStyle("ToggleMixed");
            }

            if (GUILayout.Toggle(false, s_AddressableAssetToggleText, s_toggleMixed, GUILayout.ExpandWidth(false)))
            {
                ////SetAaEntry(AddressableAssetSettingsDefaultObject.GetSettings(true), targetInfos, true);
            }

            FindUniqueAssetGuids(targetInfos, out var uniqueAssetGuids, out var uniqueAddressableAssetGuids);

            EditorGUILayout.LabelField(uniqueAddressableAssetGuids.Count + " out of " + uniqueAssetGuids.Count + " assets are addressable.");
            
            DrawSelectEntriesButton(targetInfos);

            GUILayout.EndHorizontal();
        }

        // Set the enabled state to the previous state.
        GUI.enabled = prevEnabledState;
    }

    static void SetAaEntry(AddressableAssetSettings settings, List<TargetInfo> targetInfos, bool create)
    {
        if (create && settings.DefaultGroup.ReadOnly)
        {
            Debug.LogError("Current default group is ReadOnly.  Cannot add addressable assets to it");
            return;
        }

        Undo.RecordObject(settings, "AddressableAssetSettings");

        if (!create)
        {
            List<AddressableAssetEntry> removedEntries = new List<AddressableAssetEntry>(targetInfos.Count);
            for (int i = 0; i < targetInfos.Count; ++i)
            {
                AddressableAssetEntry e = settings.FindAssetEntry(targetInfos[i].Guid);
                AddressableAssetUtility.OpenAssetIfUsingVCIntegration(e.parentGroup);

                removedEntries.Add(e);
                settings.RemoveAssetEntry(removedEntries[i], false);
            }

            if (removedEntries.Count > 0)
            {
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, removedEntries, true, false);
            }
        }
        else
        {
            AddressableAssetGroup parentGroup = settings.DefaultGroup;
            var resourceTargets = targetInfos.Where(ti => AddressableAssetUtility.IsInResources(ti.Path));
            if (resourceTargets.Any())
            {
                var resourcePaths = resourceTargets.Select(t => t.Path).ToList();
                var resourceGuids = resourceTargets.Select(t => t.Guid).ToList();
                AddressableAssetUtility.SafeMoveResourcesToGroup(settings, parentGroup, resourcePaths, resourceGuids);
            }

            var otherTargetInfos = targetInfos.Except(resourceTargets);
            List<string> otherTargetGuids = new List<string>(targetInfos.Count);
            foreach (var info in otherTargetInfos)
            {
                otherTargetGuids.Add(info.Guid);
            }

            var entriesCreated = new List<AddressableAssetEntry>();
            var entriesMoved = new List<AddressableAssetEntry>();
            settings.CreateOrMoveEntries(otherTargetGuids, parentGroup, entriesCreated, entriesMoved, false, false);

            bool openedInVC = false;
            if (entriesMoved.Count > 0)
            {
                AddressableAssetUtility.OpenAssetIfUsingVCIntegration(parentGroup);
                openedInVC = true;
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesMoved, true, false);
            }

            if (entriesCreated.Count > 0)
            {
                if (!openedInVC)
                {
                    AddressableAssetUtility.OpenAssetIfUsingVCIntegration(parentGroup);
                }

                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entriesCreated, true, false);
            }
        }
    }

    private static void DrawLabels(List<TargetInfo> entryInfos, AddressableAssetSettings aaSettings, Editor editor)
    {
        var entries = new List<AddressableAssetEntry>();
        var labelNameToFreqCount = new Dictionary<string, int>();
        var nonEditableLabels = new HashSet<string>();

        for (int i = 0; i < entryInfos.Count; ++i)
        {
            AddressableAssetEntry entry = aaSettings.FindAssetEntry(entryInfos[i].Guid);
            if (entry == null)
            {
                continue;
            }

            entries.Add(entry);

            foreach (string label in entry.labels)
            {
                labelNameToFreqCount.TryGetValue(label, out int labelCount);
                labelCount++;
                labelNameToFreqCount[label] = labelCount;

                if (entry.ReadOnly || entry.IsSubAsset)
                {
                    nonEditableLabels.Add(label);
                }
            }
        }

        int totalNumLabels = labelNameToFreqCount.Count;
        Rect rowRect = EditorGUILayout.GetControlRect(true, 0f);
        float totalRowWidth = rowRect.width; // must be called outside of Begin/EndHoriziontal scope to get correct width

        GUILayout.BeginHorizontal();

        if (s_AssetLabelIconStyle == null)
        {
            s_AssetLabelIconStyle = UnityEngine.GUI.skin.FindStyle("AssetLabel Icon") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("AssetLabel Icon");
        }

        var buttonRect = GUILayoutUtility.GetRect(s_ConfigureLabelsGUIContent, s_AssetLabelIconStyle);
        if (totalRowWidth > 1) // in some frames totalRowWidth is 1 only
            buttonRect.x = (totalRowWidth - buttonRect.width) + 3;

        GUIContent labelCountGUIContent = new GUIContent($"({totalNumLabels})");
        var labelCountRect = GUILayoutUtility.GetRect(labelCountGUIContent, EditorStyles.miniLabel);
        if (totalRowWidth > 1)
        {
            // in some frames totalRowWidth is 1 only
            labelCountRect.x = buttonRect.x - (labelCountRect.width + 2);
        }

        float xOffset = s_RemoveButtonWidth + labelCountRect.width + buttonRect.width;
        float xMax = labelCountRect.x;

        // Draw modifiable (shared) labels
        var disabledLabels = new List<string>();
        foreach (KeyValuePair<string, int> pair in labelNameToFreqCount)
        {
            string label = pair.Key;
            if (!nonEditableLabels.Contains(label) && entries.Count == labelNameToFreqCount[label])
                DrawLabel(entries, aaSettings, label, xOffset, xMax);
            else
                disabledLabels.Add(label);
        }

        // Draw disabled labels
        using (new EditorGUI.DisabledGroupScope(true))
        {
            foreach (string label in disabledLabels)
            {
                DrawLabel(entries, aaSettings, label, xOffset, xMax);
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUI.LabelField(labelCountRect, labelCountGUIContent, EditorStyles.miniLabel);
        if (EditorGUI.DropdownButton(buttonRect, s_ConfigureLabelsGUIContent, FocusType.Passive, s_AssetLabelIconStyle))
        {
            PopupWindow.Show(buttonRect, new LabelMaskPopupContent(rowRect, aaSettings, entries, labelNameToFreqCount, editor));
        }

        GUILayout.EndHorizontal();
    }

    private static void DrawLabel(List<AddressableAssetEntry> entries, AddressableAssetSettings aaSettings, string label, float xOffset, float xMax)
    {
        GUIContent labelGUIContent = GetGUIContentForLabel(label, s_MaxLabelCharCount);
        if (s_AssetLabelStyle == null)
        {
            s_AssetLabelStyle = UnityEngine.GUI.skin.FindStyle("AssetLabel") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("AssetLabel");
        }

        Rect labelRect = GUILayoutUtility.GetRect(labelGUIContent, s_AssetLabelStyle);

        labelRect.x -= xOffset;
        if (labelRect.xMax + s_RemoveButtonWidth < xMax)
        {
            EditorGUI.LabelField(labelRect, labelGUIContent, s_AssetLabelStyle);

            if (s_AssetLabelXButtonStyle == null)
            {
                s_AssetLabelXButtonStyle = UnityEngine.GUI.skin.FindStyle("IconButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("IconButton");
            }

            Rect removeButtonRect = GUILayoutUtility.GetRect(s_RemoveButtonWidth, s_RemoveButtonWidth, s_AssetLabelXButtonStyle);
            removeButtonRect.x = labelRect.xMax - 4; // overlap the button on the pill-shaped label
            if (EditorGUI.DropdownButton(removeButtonRect, s_RemoveButtonGUIContent, FocusType.Passive, s_AssetLabelXButtonStyle))
            {
                aaSettings.SetLabelValueForEntries(entries, label, false);
            }
        }
    }

    private static GUIContent GetGUIContentForLabel(string labelName, int charCount)
    {
        string displayText;
        int maxLabelCharCount = charCount - 3; // account for length of "..."
        if (labelName.Length > maxLabelCharCount)
            displayText = labelName.Substring(0, maxLabelCharCount) + "...";
        else
            displayText = labelName;
        return new GUIContent(displayText, labelName);
    }

    internal static void FindUniqueAssetGuids(List<TargetInfo> targetInfos, out HashSet<string> uniqueAssetGuids, out HashSet<string> uniqueAddressableAssetGuids)
    {
        uniqueAssetGuids = new HashSet<string>();
        uniqueAddressableAssetGuids = new HashSet<string>();
        foreach (TargetInfo info in targetInfos)
        {
            uniqueAssetGuids.Add(info.Guid);
            if (info.MainAssetEntry != null)
                uniqueAddressableAssetGuids.Add(info.Guid);
        }
    }

    static void DrawSelectEntriesButton(List<TargetInfo> targets)
    {
        var prevGuiEnabled = UnityEngine.GUI.enabled;
        UnityEngine.GUI.enabled = true;

        if (GUILayout.Button("Select"))
        {
            ////AddressableAssetsWindow.Init();
            ////var window = EditorWindow.GetWindow<AddressableAssetsWindow>();
            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>(targets.Count);
            foreach (TargetInfo info in targets)
            {
                if (info.MainAssetEntry != null)
                {
                    if (info.IsMainAsset == false && ProjectConfigData.ShowSubObjectsInGroupView)
                    {
                        List<AddressableAssetEntry> subs = new List<AddressableAssetEntry>();
                        info.MainAssetEntry.GatherAllAssets(subs, false, true, true);
                        foreach (AddressableAssetEntry sub in subs)
                        {
                            if (sub.TargetAsset == info.TargetObject)
                            {
                                entries.Add(sub);
                                break;
                            }
                        }
                    }
                    else
                    {
                        entries.Add(info.MainAssetEntry);
                    }
                }
            }

            if (entries.Count > 0)
            {
                ////window.SelectAssetsInGroupEditor(entries);
            }
        }

        GUI.enabled = prevGuiEnabled;
    }

    static void DrawGroupsDropdown(AddressableAssetSettings settings, List<TargetInfo> targets)
    {
        bool enabledDropdown = true;
        bool mixedValueDropdown = false;
        AddressableAssetGroup displayGroup = null;
        var entries = new List<AddressableAssetEntry>();
        foreach (TargetInfo info in targets)
        {
            AddressableAssetEntry entry = info.MainAssetEntry;
            if (entry == null)
            {
                enabledDropdown = false;
            }
            else
            {
                entries.Add(entry);
                if (entry.ReadOnly || entry.parentGroup.ReadOnly)
                {
                    enabledDropdown = false;
                }

                if (displayGroup == null)
                    displayGroup = entry.parentGroup;
                else if (entry.parentGroup != displayGroup)
                {
                    mixedValueDropdown = true;
                }
            }
        }

        ////GroupsPopupUtility.DrawGroupsDropdown(s_GroupsDropdownLabelContent, displayGroup, enabledDropdown, mixedValueDropdown, false, AddressableAssetUtility.MoveEntriesToGroup, entries);
    }

    #endregion
}
//*/
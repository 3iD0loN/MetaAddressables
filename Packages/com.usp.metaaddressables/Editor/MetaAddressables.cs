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

public class MetaAddressables
{
    #region Constants
    private const string UserDataKey = "MetaAddressables";
    #endregion

    #region Types
    [Serializable]
    public class AssetData
    {
        #region Static Methods
        public static string SimplifyAddress(string address)
        {
            return Path.GetFileNameWithoutExtension(address);
        }
        #endregion

        #region Fields
        [SerializeField]
        string _guid;

        [SerializeField]
        private string _address;

        [SerializeField]
        private bool _readOnly;

        [SerializeField]
        private List<string> _labels;

        [SerializeField]
        private bool _flaggedDuringContentUpdateRestriction = false;
        #endregion

        #region Properties
        public string Guid
        {
            get
            {
                return _guid;
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
            }
        }

        public List<string> Labels
        {
            get
            {
                return _labels;
            }
            set
            {
                _labels = value;
            }
        }

        public bool FlaggedDuringContentUpdateRestriction
        {
            get
            {
                return _flaggedDuringContentUpdateRestriction;
            }
        }
        #endregion

        #region Methods
        public AssetData(string guid, string address, bool readOnly)
        {
            _guid = guid;

            _address = address;
            
            _readOnly = readOnly;
            
            _labels = new List<string>();
        }

        public AssetData(AddressableAssetEntry entry)
        {
            _guid = entry.guid;

            _address = entry.address;

            _readOnly = entry.ReadOnly;

            _labels = entry.labels.ToList();
        }
        #endregion
    }

    public interface IGroupSchemaData
    {
        int GetHashCode();
    }

    [Serializable]
    public class BundledAssetGroupSchemaData : IGroupSchemaData
    {
        #region Fields
        [SerializeField]
        private BundleInternalIdMode _internalBundleIdMode = BundleInternalIdMode.GroupGuidProjectIdHash;

        [SerializeField]
        private BundleCompressionMode _compression = BundleCompressionMode.LZ4;

        [SerializeField]
        private bool _includeAddressInCatalog = true;

        [SerializeField]
        private bool _includeGUIDInCatalog = true;

        [SerializeField]
        private bool _includeLabelsInCatalog = true;

        [SerializeField]
        private AssetNamingMode _internalIdNamingMode = AssetNamingMode.FullPath;

        [SerializeField]
        private CacheClearBehavior _cacheClearBehavior = CacheClearBehavior.ClearWhenSpaceIsNeededInCache;

        [SerializeField]
        private bool _includeInBuild = true;

        [SerializeField]
        [SerializedTypeRestriction(type = typeof(IResourceProvider))]
        private SerializedType _bundledAssetProviderType;

        [SerializeField]
        private bool _forceUniqueProvider = false;

        [SerializeField]
        private bool _useAssetBundleCache = true;

        [SerializeField]
        private bool _useAssetBundleCrc = true;

        [SerializeField]
        private bool _useAssetBundleCrcForCachedBundles = true;

        [SerializeField]
        private bool _useUWRForLocalBundles = false;

        [SerializeField]
        [Min(0)]
        private int _timeout;

        [SerializeField]
        private bool _chunkedTransfer;

        [SerializeField]
        [Range(-1, 128)]
        private int _redirectLimit = -1;

        [SerializeField]
        [Range(0, 128)]
        private int _retryCount = 0;

        [SerializeField]
        private ProfileValueReference _buildPath = new ProfileValueReference();

        [SerializeField]
        private ProfileValueReference _loadPath = new ProfileValueReference();

        [SerializeField]
        private BundlePackingMode _bundleMode = BundlePackingMode.PackTogether;

        [SerializeField]
        [SerializedTypeRestriction(type = typeof(IResourceProvider))]
        private SerializedType _assetBundleProviderType;

        [SerializeField]
        private bool _useDefaultSchemaSettings;

        [SerializeField]
        private int _selectedPathPairIndex;

        [SerializeField]
        private BundleNamingStyle _bundleNaming;

        [SerializeField]
        private AssetLoadMode _assetLoadMode;
        #endregion

        #region Properties
        public BundleInternalIdMode InternalBundleIdMode
        {
            get
            {
                return _internalBundleIdMode;
            }
        }

        public BundleCompressionMode Compression
        {
            get
            {
                return _compression;
            }
        }

        public bool IncludeAddressInCatalog
        {
            get
            {
                return _includeAddressInCatalog;
            }
        }

        public bool IncludeGUIDInCatalog
        {
            get
            {
                return _includeGUIDInCatalog;
            }
        }

        public bool IncludeLabelsInCatalog
        {
            get
            {
                return _includeLabelsInCatalog;
            }
        }

        public AssetNamingMode InternalIdNamingMode
        {
            get
            {
                return _internalIdNamingMode;
            }
        }

        public CacheClearBehavior CacheClearBehavior
        {
            get
            {
                return _cacheClearBehavior;
            }
        }

        public bool IncludeInBuild
        {
            get
            {
                return _includeInBuild;
            }
        }

        public SerializedType BundledAssetProviderType
        {
            get
            {
                return _bundledAssetProviderType;
            }
        }

        public bool ForceUniqueProvider
        {
            get
            {
                return _forceUniqueProvider;
            }
        }

        public bool UseAssetBundleCache
        {
            get
            {
                return _useAssetBundleCache;
            }
        }

        public bool UseAssetBundleCrc
        {
            get
            {
                return _useAssetBundleCrc;
            }
        }

        public bool UseAssetBundleCrcForCachedBundles
        {
            get
            {
                return _useAssetBundleCrcForCachedBundles;
            }
        }

        public bool UseUWRForLocalBundles
        {
            get
            {
                return _useUWRForLocalBundles;
            }
        }

        public int Timeout
        {
            get
            {
                return _timeout;
            }
        }

        public bool ChunkedTransfer
        {
            get
            {
                return _chunkedTransfer;
            }
        }

        public int RedirectLimit
        {
            get
            {
                return _redirectLimit;
            }
        }

        public int RetryCount
        {
            get
            {
                return _retryCount;
            }
        }

        public ProfileValueReference BuildPath
        {
            get
            {
                return _buildPath;
            }
        }

        public ProfileValueReference LoadPath
        {
            get
            {
                return _loadPath;
            }
        }

        public BundlePackingMode BundleMode
        {
            get
            {
                return _bundleMode;
            }
        }

        public SerializedType AssetBundleProviderType
        {
            get
            {
                return _assetBundleProviderType;
            }
        }

        public bool UseDefaultSchemaSettings
        {
            get
            {
                return _useDefaultSchemaSettings;
            }
        }

        public int SelectedPathPairIndex
        {
            get
            {
                return _selectedPathPairIndex;
            }
        }

        public BundleNamingStyle BundleNaming
        {
            get
            {
                return _bundleNaming;
            }
        }

        public AssetLoadMode AssetLoadMode
        {
            get
            {
                return _assetLoadMode;
            }
        }
        #endregion

        #region Methods
        public BundledAssetGroupSchemaData(BundledAssetGroupSchema schema)
        {
            _internalBundleIdMode = schema.InternalBundleIdMode;
            _compression = schema.Compression;
            _includeAddressInCatalog = schema.IncludeAddressInCatalog;
            _includeGUIDInCatalog = schema.IncludeGUIDInCatalog;
            _includeLabelsInCatalog = schema.IncludeLabelsInCatalog;
            _internalIdNamingMode = schema.InternalIdNamingMode;
            _cacheClearBehavior = schema.AssetBundledCacheClearBehavior;
            _includeInBuild = schema.IncludeInBuild;
            _bundledAssetProviderType = schema.AssetBundleProviderType;
            _forceUniqueProvider = schema.ForceUniqueProvider;
            _useAssetBundleCache = schema.UseAssetBundleCache;
            _useAssetBundleCrc = schema.UseAssetBundleCrc;
            _useAssetBundleCrcForCachedBundles = schema.UseAssetBundleCrcForCachedBundles;
            _useUWRForLocalBundles = schema.UseUnityWebRequestForLocalBundles;
            _timeout = schema.Timeout;
            _chunkedTransfer = schema.ChunkedTransfer;
            _redirectLimit = schema.RedirectLimit;
            _retryCount = schema.RetryCount;
            _buildPath = schema.BuildPath;
            _loadPath = schema.LoadPath;
            _bundleMode = schema.BundleMode;
            _assetBundleProviderType = schema.AssetBundleProviderType;
            _useDefaultSchemaSettings = schema.UseDefaultSchemaSettings;
            _selectedPathPairIndex = schema.SelectedPathPairIndex;
            _bundleNaming = schema.BundleNaming;
            _assetLoadMode = schema.AssetLoadMode;
        }

        public override int GetHashCode()
        {
            int result = _internalBundleIdMode.GetHashCode();
            result = result * 31 ^ _compression.GetHashCode();
            result = result * 31 ^ _includeAddressInCatalog.GetHashCode();
            result = result * 31 ^ _includeGUIDInCatalog.GetHashCode();
            result = result * 31 ^ _includeLabelsInCatalog.GetHashCode();
            result = result * 31 ^ _internalIdNamingMode.GetHashCode();
            result = result * 31 ^ _cacheClearBehavior.GetHashCode();
            result = result * 31 ^ _includeInBuild.GetHashCode();
            result = result * 31 ^ _bundledAssetProviderType.Value.GetHashCode();
            result = result * 31 ^ _forceUniqueProvider.GetHashCode();
            result = result * 31 ^ _useAssetBundleCache.GetHashCode();
            result = result * 31 ^ _useAssetBundleCrc.GetHashCode();
            result = result * 31 ^ _useAssetBundleCrcForCachedBundles.GetHashCode();
            result = result * 31 ^ _useUWRForLocalBundles.GetHashCode();
            result = result * 31 ^ _timeout.GetHashCode();
            result = result * 31 ^ _chunkedTransfer.GetHashCode();
            result = result * 31 ^ _redirectLimit.GetHashCode();
            result = result * 31 ^ _retryCount.GetHashCode();
            result = result * 31 ^ _buildPath.Id.GetHashCode();
            result = result * 31 ^ _loadPath.Id.GetHashCode();
            result = result * 31 ^ _bundleMode.GetHashCode();
            result = result * 31 ^ _assetBundleProviderType.Value.GetHashCode();            
            result = result * 31 ^ _useDefaultSchemaSettings.GetHashCode();
            result = result * 31 ^ _selectedPathPairIndex.GetHashCode();
            result = result * 31 ^ _bundleNaming.GetHashCode();
            result = result * 31 ^ _assetLoadMode.GetHashCode();

            return result;
        }
        #endregion
    }

    [Serializable]
    public class ContentUpdateGroupSchemaData : IGroupSchemaData
    {
        #region Fields
        [SerializeField]
        private bool _staticContent;
        #endregion

        #region Properties
        public bool StaticContent
        {
            get
            {
                return _staticContent;
            }
        }
        #endregion

        #region Methods
        public ContentUpdateGroupSchemaData(ContentUpdateGroupSchema schema)
        {
            _staticContent = schema.StaticContent;
        }

        public override int GetHashCode()
        {
            return _staticContent.GetHashCode();
        }
        #endregion
    }

    [Serializable]
    public class GroupData
    {
        #region Static Methods
        private static IGroupSchemaData CreateSchemaData(AddressableAssetGroupSchema groupSchema)
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

        private static AddressableAssetGroupSchema CreateSchema(IGroupSchemaData groupSchemaData)
        {
            if (groupSchemaData is BundledAssetGroupSchemaData bundledAssetGroupScema)
            {
                var settings = AddressableAssetSettingsDefaultObject.Settings;

                var groupSchema = new BundledAssetGroupSchema();

                groupSchema.InternalBundleIdMode = bundledAssetGroupScema.InternalBundleIdMode;
                groupSchema.Compression = bundledAssetGroupScema.Compression;
                groupSchema.IncludeAddressInCatalog = bundledAssetGroupScema.IncludeAddressInCatalog;
                groupSchema.IncludeGUIDInCatalog = bundledAssetGroupScema.IncludeGUIDInCatalog;
                groupSchema.IncludeLabelsInCatalog = bundledAssetGroupScema.IncludeLabelsInCatalog;
                groupSchema.InternalIdNamingMode = bundledAssetGroupScema.InternalIdNamingMode;
                groupSchema.AssetBundledCacheClearBehavior = bundledAssetGroupScema.CacheClearBehavior;
                groupSchema.IncludeInBuild = bundledAssetGroupScema.IncludeInBuild;
                groupSchema.BundledAssetProviderType = bundledAssetGroupScema.AssetBundleProviderType;
                groupSchema.ForceUniqueProvider = bundledAssetGroupScema.ForceUniqueProvider;
                groupSchema.UseAssetBundleCache = bundledAssetGroupScema.UseAssetBundleCache;
                groupSchema.UseAssetBundleCrc = bundledAssetGroupScema.UseAssetBundleCrc;
                groupSchema.UseAssetBundleCrcForCachedBundles = bundledAssetGroupScema.UseAssetBundleCrcForCachedBundles;
                groupSchema.UseUnityWebRequestForLocalBundles = bundledAssetGroupScema.UseUWRForLocalBundles;
                groupSchema.Timeout = bundledAssetGroupScema.Timeout;
                groupSchema.ChunkedTransfer = bundledAssetGroupScema.ChunkedTransfer;
                groupSchema.RedirectLimit = bundledAssetGroupScema.RedirectLimit;
                groupSchema.RetryCount = bundledAssetGroupScema.RetryCount;
                groupSchema.BuildPath.SetVariableById(settings, bundledAssetGroupScema.BuildPath.Id);
                groupSchema.LoadPath.SetVariableById(settings, bundledAssetGroupScema.LoadPath.Id);
                groupSchema.BundleMode = bundledAssetGroupScema.BundleMode;
                groupSchema.AssetBundleProviderType = bundledAssetGroupScema.AssetBundleProviderType;
                groupSchema.UseDefaultSchemaSettings = bundledAssetGroupScema.UseDefaultSchemaSettings;
                groupSchema.SelectedPathPairIndex = bundledAssetGroupScema.SelectedPathPairIndex;
                groupSchema.BundleNaming = bundledAssetGroupScema.BundleNaming;
                groupSchema.AssetLoadMode = bundledAssetGroupScema.AssetLoadMode;

                return groupSchema;
            }
            else if (groupSchemaData is ContentUpdateGroupSchemaData contentUpdateGroupSchema)
            {
                var groupSchema = new ContentUpdateGroupSchema();
                groupSchema.StaticContent = contentUpdateGroupSchema.StaticContent;

                return groupSchema;
            }

            return null;
        }
        #endregion

        #region Fields
        [SerializeField]
        private string _guid;

        [SerializeField]
        private bool _readOnly;

        [SerializeReference]
        private List<IGroupSchemaData> _schemaData;
        #endregion

        #region Properties
        public string Guid
        {
            get
            {
                return _guid;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        public List<IGroupSchemaData> SchemaData
        {
            get
            {
                return _schemaData;
            }
        }
        #endregion

        #region Methods
        public GroupData(AddressableAssetGroupTemplate groupTemplate) :
            this(string.Empty, false, groupTemplate.SchemaObjects)
        {
        }

        public GroupData(AddressableAssetGroup group) :
            this(group.Guid, group.ReadOnly, group.Schemas)
        {}

        private GroupData(string guid, bool readOnly, List<AddressableAssetGroupSchema> groupSchemas)
        {
            _guid = guid;

            _readOnly = readOnly;

            _schemaData = new List<IGroupSchemaData>();
            foreach (AddressableAssetGroupSchema groupSchema in groupSchemas)
            {
                var data = CreateSchemaData(groupSchema);
                _schemaData.Add(data);
            }
        }

        public override int GetHashCode()
        {
            int result = _readOnly.GetHashCode();
            foreach(var schema in _schemaData)
            {
                int hash = schema.GetHashCode();
                result = result * 31 ^ hash;
            }

            return result;
        }
        public List<AddressableAssetGroupSchema> CreateSchemas()
        {
            var result = new List<AddressableAssetGroupSchema>(SchemaData.Count);
            foreach (IGroupSchemaData schemaData in SchemaData)
            {
                AddressableAssetGroupSchema schema = CreateSchema(schemaData);
                result.Add(schema);
            }

            return result;
        }
        #endregion
    }

    [Serializable]
    public class UserData
    {
        #region Fields
        [SerializeField]
        private AssetData _asset;

        [SerializeField]
        private GroupData _group;
        #endregion

        #region Properties
        public AssetData Asset
        {
            get { return _asset; }
            set { _asset = value; }
        }

        public GroupData Group
        {
            get { return _group; }
            set { _group = value; }
        }
        #endregion

        #region Methods
        public UserData(AddressableAssetEntry entry) :
            this(new AssetData(entry), new GroupData(entry.parentGroup))
        {
        }

        public UserData(AssetData asset, GroupData group)
        {
            _asset = asset;
            _group = group;
        }
        #endregion
    }

    public class Factory
    {
        #region Properties
        public AddressableAssetGroupTemplate LocalGroupTemplate
        {
            get;
            set;
        }

        public AddressableAssetGroupTemplate ActiveGroupTemplate
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public UserData Create(AssetImporter assetImporter)
        {
            // Get the asset GUID associated with the asset file path.
            GUID guid = AssetDatabase.GUIDFromAssetPath(assetImporter.assetPath);

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // Attempt to find an Addressable asset entry that is associated with the asset GUID.
            // If there is, then the asset is already Addressable.
            AddressableAssetEntry entry = settings.FindAssetEntry(guid.ToString());

            // If the asset is already Addressable, then: 
            if (entry != null)
            {
                return new UserData(entry);
            }

            var assetData = new AssetData(guid.ToString(), assetImporter.assetPath, false);
            var groupData = new GroupData(ActiveGroupTemplate);

            return new UserData(assetData, groupData);
        }
        #endregion
    }
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

        foreach(var group in settings.groups)
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
            s_groupsByGuids.TryGetValue(groupData.Guid, out group);
            return group;
        }

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

    public static void Write(string assetFilePath, UserData userData)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetFilePath);

        MetaFile.Write(assetImporter, UserDataKey, userData);
    }
    #endregion
}

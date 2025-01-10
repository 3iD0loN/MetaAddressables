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

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
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
        #endregion
    }
}

using System;

using UnityEngine;

using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace USP.MetaAddressables
{
    public static partial class MetaAddressables
    {
        #region Types
        [Serializable]
        public class ContentUpdateGroupSchemaData : GroupSchemaData
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

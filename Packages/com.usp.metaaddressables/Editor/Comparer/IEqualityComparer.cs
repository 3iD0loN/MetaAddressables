using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using UnityEngine.ResourceManagement.Util;

using static UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema;
using static UnityEngine.GraphicsBuffer;

namespace USP.MetaAddressables
{
    public interface IEqualityComparer
    {
        #region Methods
        bool Equals(object x, object y);

        int GetHashCode(object obj);
        #endregion
    }
}

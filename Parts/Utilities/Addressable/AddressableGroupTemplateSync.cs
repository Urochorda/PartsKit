using System;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace PartsKit
{
    [CreateAssetMenu(menuName = "PartsKit/Addressable/GroupTemplateSync", fileName = "AddressableGroupTemplateSync")]
    public class AddressableGroupTemplateSync : ScriptableObject
    {
        [Serializable]
        private class GroupData
        {
            [SerializeField] private AddressableAssetGroup assetGroup;
            [SerializeField] private AddressableAssetGroupTemplate assetGroupTemplate;

            public AddressableAssetGroup AssetGroup => assetGroup;
            public AddressableAssetGroupTemplate AssetGroupTemplate => assetGroupTemplate;
        }

        [SerializeField] private List<GroupData> groups;

        public void SyncTemplate()
        {
            foreach (var data in groups)
            {
                if (data.AssetGroup == null || data.AssetGroupTemplate == null)
                {
                    continue;
                }

                data.AssetGroupTemplate.ApplyToAddressableAssetGroup(data.AssetGroup);
            }
        }

        public void ClearMiss()
        {
            for (var i = groups.Count - 1; i >= 0; i--)
            {
                var data = groups[i];
                if (data.AssetGroup != null)
                {
                    continue;
                }

                groups.RemoveAt(i);
            }
        }
    }
}
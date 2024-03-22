using System.Collections.Generic;
using UnityEngine;

namespace SimpleResourcesSystem.SimpleItemSystem
{
    public static class SimpleResourcesDatabase
    {
        public static List<BaseResourceInfo> ResourcesInfo
        {
            get
            {
                if (ResourcesInfo.Count == 0)
                    ResourcesInfo.AddRange(Resources.LoadAll<BaseResourceInfo>("Assets/SimpleInventorySystem/Resources/SimpleResources"));

                Debug.Log($"Load In Resources: {ResourcesInfo.Count}");
                return ResourcesInfo;
            }
        }
    }
}
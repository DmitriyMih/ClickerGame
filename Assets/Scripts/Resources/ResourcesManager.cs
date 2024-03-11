using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Resources
{
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        [SerializeField] private List<ResourceItemInfo> availableResourcesInfo = new();
        public Dictionary<ResourceType, ResourceItemInfo> StoredResourcesInfo { get; private set; } = new();
        public Dictionary<ResourceType, int> StoredResources { get; private set; } = new();

        public event Action<ResourceType, int> OnResourcesChanged;

        protected override void Awake()
        {
            base.Awake();
            Inititalization();
        }

        private void Inititalization()
        {
            for (int i = 0; i < availableResourcesInfo.Count; i++)
            {
                ResourceType resourcesType = availableResourcesInfo[i].ResourcesType;
                if (StoredResources.ContainsKey(resourcesType))
                {
                    Debug.LogError($"Resources {resourcesType} Is Already Initialized In Dictionary");
                    continue;
                }

                int resourceValue = ResourcesSave.LoadResource(availableResourcesInfo[i].ResourcesType);

                StoredResources.Add(resourcesType, resourceValue);
                StoredResourcesInfo.Add(resourcesType, availableResourcesInfo[i]);
                OnResourcesChanged?.Invoke(resourcesType, resourceValue);
            }
        }

        public void AddResource(ResourceType resourcesType, int value)
        {
            if (!StoredResources.ContainsKey(resourcesType))
            {
                Debug.LogError($"Resources {resourcesType} Not Available");
                return;
            }
            //Debug.Log($"Add Resource {resourcesType} | {value}");

            value = Mathf.Clamp(value, 0, int.MaxValue);
            StoredResources[resourcesType] = Mathf.Clamp(StoredResources[resourcesType] + value, 0, int.MaxValue);

            OnResourcesChanged?.Invoke(resourcesType, StoredResources[resourcesType]);

            ResourcesSave.SaveResource(resourcesType, StoredResources[resourcesType]);
        }
    }
}
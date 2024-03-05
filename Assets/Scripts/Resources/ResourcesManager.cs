using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Resources
{
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        [SerializeField] private List<ResourceItemInfo> resourcesInfo = new();
        [SerializeField] private ResourcesViewController resourcesView;

        public Dictionary<ResourceType, int> ResourceInfoDictionary { get; private set; } = new();
        public event Action<ResourceType, int> OnResourcesChanged;

        protected override void Awake()
        {
            base.Awake();
            Inititalization();
        }

        private void Inititalization()
        {
            Dictionary<ResourceItemInfo, int> recourcesTempInfo = new();

            for (int i = 0; i < resourcesInfo.Count; i++)
            {
                ResourceType resourcesType = resourcesInfo[i].ResourcesType;
                if (ResourceInfoDictionary.ContainsKey(resourcesType))
                {
                    Debug.LogError($"Resources {resourcesType} Is Already Initialized In Dictionary");
                    continue;
                }

                int resourceValue = RecourcesSave.LoadResource(resourcesInfo[i].ResourcesType);
                ResourceInfoDictionary.Add(resourcesType, resourceValue);

                recourcesTempInfo.Add(resourcesInfo[i], resourceValue);
            }

            if (resourcesView != null)
                resourcesView.Inititalization(this, recourcesTempInfo);
        }

        public static void AddResource(ResourceType resourcesType, int value)
        {
            if (Instance == null) return;

            value = Mathf.Clamp(value, 0, int.MaxValue);
            if (value <= 0) return;

            Instance.ResourceInfoDictionary[resourcesType] += value;
            Instance.OnResourcesChanged?.Invoke(resourcesType, Instance.ResourceInfoDictionary[resourcesType]);

            RecourcesSave.SaveResource(resourcesType, Instance.ResourceInfoDictionary[resourcesType]);
        }
    }
}
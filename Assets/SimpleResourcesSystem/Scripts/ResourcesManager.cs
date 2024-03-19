using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleResourcesSystem
{
    public enum StatusState
    {
        Success,
        Failed
    }

    public class ResourcesManager : MonoBehaviour
    {
        [field: SerializeField] public int ManagerIndex { get; private set; }

        [SerializeField] private List<ResourceInfo> availableResourcesInfo = new();

        [Space(), Header("Debug Settings")]
        [SerializeField] private bool showOutput;

        public Dictionary<string, ResourceInfo> StoredResourcesInfo { get; private set; } = new();
        public Dictionary<string, int> StoredResources { get; private set; } = new();

        public event Action<ResourceInfo, int> OnResourcesChanged;

        private void Awake()
        {
            Inititalization();
        }

        private void Inititalization()
        {
            for (int i = 0; i < availableResourcesInfo.Count; i++)
            {
                ResourceInfo resourceInfo = availableResourcesInfo[i];
                if (StoredResources.ContainsKey(resourceInfo.ResourcesKey))
                {
                    Debug.LogError($"Resources {resourceInfo} Is Already Initialized In Dictionary");
                    continue;
                }

                int resourceValue = ResourcesSave.LoadResource(availableResourcesInfo[i].ResourcesKey, ManagerIndex);

                StoredResources.Add(resourceInfo.ResourcesKey, resourceValue);
                StoredResourcesInfo.Add(resourceInfo.ResourcesKey, availableResourcesInfo[i]);
                OnResourcesChanged?.Invoke(resourceInfo, resourceValue);
            }
        }

        public bool HasResourcesAmount(string resourceKey, int value)
        {
            if (!StoredResources.ContainsKey(resourceKey))
            {
                Debug.LogError($"Resources {resourceKey} Not Available");
                return false;
            }

            return StoredResources[resourceKey] - value >= 0;
        }

        public void ChangeResource(string resourceKey, int value, ActionType actionType, Action<StatusState> callback = null)
        {
            if (!StoredResources.ContainsKey(resourceKey))
            {
                Debug.LogError($"Resources {resourceKey} Not Available");
                callback?.Invoke(StatusState.Failed);
                return;
            }

            if (actionType == ActionType.Remove && !HasResourcesAmount(resourceKey, value))
            {
                Debug.Log($"Not Enought {resourceKey} | Available {StoredResources[resourceKey]} / Try Get {value}");
                callback?.Invoke(StatusState.Failed);
                return;
            }

            int newResourceValue = value * (actionType == ActionType.Add ? 1 : -1);
            StoredResources[resourceKey] = Mathf.Clamp(StoredResources[resourceKey] + newResourceValue, 0, int.MaxValue);

            Debug.Log($"Add Resource {resourceKey} | {newResourceValue} || {gameObject} / {StoredResources[resourceKey]}");
            OnResourcesChanged?.Invoke(StoredResourcesInfo[resourceKey], StoredResources[resourceKey]);

            ResourcesSave.SaveResource(resourceKey, ManagerIndex, StoredResources[resourceKey]);
            callback?.Invoke(StatusState.Success);
        }
    }
}
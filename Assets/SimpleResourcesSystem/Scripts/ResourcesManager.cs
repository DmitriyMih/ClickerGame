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
                OutputInfo($"Resources {resourceKey} Not Available");
                return false;
            }

            return StoredResources[resourceKey] - value >= 0;
        }

        public void SetResource(string resourceKey, int value, ActionType actionType, Action<StatusState> action = null)
        {
            if (!StoredResources.ContainsKey(resourceKey))
            {
                OutputInfo($"Resources {resourceKey} Not Available");
                action?.Invoke(StatusState.Failed);
                return;
            }

            if (actionType == ActionType.Remove && !HasResourcesAmount(resourceKey, value))
            {
                action?.Invoke(StatusState.Failed);
                return;
            }

            int newResourceValue = Mathf.Clamp(StoredResources[resourceKey] + value * (actionType == ActionType.Add ? 1 : -1), 0, int.MaxValue);
            OutputInfo($"Set Resource {resourceKey} | {newResourceValue}");

            StoredResources[resourceKey] = newResourceValue;
            OnResourcesChanged?.Invoke(StoredResourcesInfo[resourceKey], newResourceValue);

            ResourcesSave.SaveResource(resourceKey, ManagerIndex, newResourceValue);
            action?.Invoke(StatusState.Success);
        }

        private void OutputInfo(string newOutput)
        {
            if (!showOutput) return;
            Debug.Log(newOutput);
        }
    }
}
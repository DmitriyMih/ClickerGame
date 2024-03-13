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

    public enum ActionType
    {
        Add,
        Remove
    }

    public class ResourcesManager : MonoBehaviour
    {
        [SerializeField] private List<ResourceItemInfo> availableResourcesInfo = new();

        [Space(), Header("Debug Settings")]
        [SerializeField] private bool showOutput;

        public Dictionary<ResourceType, ResourceItemInfo> StoredResourcesInfo { get; private set; } = new();
        public Dictionary<ResourceType, int> StoredResources { get; private set; } = new();

        public event Action<ResourceType, int> OnResourcesChanged;

        private void Awake()
        {
            Inititalization();
        }

        private void Inititalization()
        {
            for (int i = 0; i < availableResourcesInfo.Count; i++)
            {
                ResourceType resourcesType = availableResourcesInfo[i].ResourcesType;
                if (StoredResources.ContainsKey(resourcesType))
                {
                    OutputInfo($"Resources {resourcesType} Is Already Initialized In Dictionary");
                    continue;
                }

                int resourceValue = ResourcesSave.LoadResource(availableResourcesInfo[i].ResourcesType);

                StoredResources.Add(resourcesType, resourceValue);
                StoredResourcesInfo.Add(resourcesType, availableResourcesInfo[i]);
                OnResourcesChanged?.Invoke(resourcesType, resourceValue);
            }
        }

        public bool HasResourcesAmount(ResourceType resourcesType, int value)
        {
            if (!StoredResources.ContainsKey(resourcesType))
            {
                OutputInfo($"Resources {resourcesType} Not Available");
                return false;
            }

            return StoredResources[resourcesType] - value >= 0;
        }

        public void SetResource(ResourceType resourcesType, int value, ActionType actionType, Action<StatusState> action = null)
        {
            if (!StoredResources.ContainsKey(resourcesType))
            {
                OutputInfo($"Resources {resourcesType} Not Available");
                action?.Invoke(StatusState.Failed);
                return;
            }

            if (actionType == ActionType.Remove && !HasResourcesAmount(resourcesType, value))
            {
                action?.Invoke(StatusState.Failed);
                return;
            }

            int newResourceValue = Mathf.Clamp(StoredResources[resourcesType] + value * (actionType == ActionType.Add ? 1 : -1), 0, int.MaxValue);
            OutputInfo($"Set Resource {resourcesType} | {newResourceValue}");

            StoredResources[resourcesType] = newResourceValue;
            OnResourcesChanged?.Invoke(resourcesType, newResourceValue);

            ResourcesSave.SaveResource(resourcesType, newResourceValue);
            action?.Invoke(StatusState.Success);
        }

        private void OutputInfo(string newOutput)
        {
            if (!showOutput) return;
            Debug.Log(newOutput);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleResourcesSystem.DemoSamples
{
    [RequireComponent(typeof(ResourcesManager))]
    public class ResourcesGenerationManager : MonoBehaviour
    {
        private ResourcesManager resourcesManager;

        [Space(), Header("Debug Settings")]
        [SerializeField] private bool showOutput;

        public Dictionary<string, ResourceGenerationInfo> StoredGenerationInfo { get; private set; } = new();
        public Dictionary<string, int> StoredResourcesGeneration { get; private set; } = new();
        private Dictionary<string, Coroutine> coroutineDictionary = new();

        public event Action<ResourceGenerationInfo, int, float> OnGenerationChanged;

        private void Awake()
        {
            resourcesManager = GetComponent<ResourcesManager>();
        }

        private void Start()
        {
            Inititalization();
        }

        private void Inititalization()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                Debug.Log(resourcesManager.StoredResourcesInfo.ElementAt(i).Value as ResourceGenerationInfo);
                if (resourcesManager.StoredResourcesInfo.ElementAt(i).Value is ResourceGenerationInfo resourceInfo)
                {
                    StoredGenerationInfo.Add(resourceInfo.ResourcesKey, resourceInfo);
                    StoredResourcesGeneration.Add(resourceInfo.ResourcesKey, ResourceGenerationSave.LoadGenerationValue(resourceInfo.ResourcesKey, 1));
                    SetGenerationState(resourceInfo.ResourcesKey, StoredResourcesGeneration[resourceInfo.ResourcesKey] > 0, resourceInfo.GenerationTime);

                    OnGenerationChanged?.Invoke(resourceInfo, StoredResourcesGeneration[resourceInfo.ResourcesKey], resourceInfo.GenerationTime);
                }
            }
        }

        private void SetGenerationState(string generationKey, bool state, float generationTime)
        {
            Debug.Log($"Set {generationKey} | {state}");
            if (coroutineDictionary.ContainsKey(generationKey))
                StopCoroutine(coroutineDictionary[generationKey]);

            if (!state)
                return;

            Coroutine coroutine = StartCoroutine(ResourceGeneration(generationKey, generationTime));
            coroutineDictionary.Add(generationKey, coroutine);
        }

        public bool HasGenerationAmount(string generationKey, int value)
        {
            if (!StoredResourcesGeneration.ContainsKey(generationKey))
            {
                Debug.LogError($"Resources {generationKey} Not Available");
                return false;
            }

            return StoredResourcesGeneration[generationKey] - value >= 0;
        }

        public void SetResourceGeneration(string generationKey, int value, ActionType actionType)
        {
            if (!StoredResourcesGeneration.ContainsKey(generationKey))
            {
                Debug.LogError($"Resources {generationKey} Not Available");
                return;
            } 

            if (actionType == ActionType.Remove && !HasGenerationAmount(generationKey, value))
                return;

            int newResourceValue = value * (actionType == ActionType.Add ? 1 : -1);
            StoredResourcesGeneration[generationKey] = Mathf.Clamp(StoredResourcesGeneration[generationKey] + newResourceValue, 0, int.MaxValue);
     
            Debug.Log($"Add Generation {generationKey} | {newResourceValue} || {gameObject} / {StoredResourcesGeneration[generationKey]}");
            OnGenerationChanged?.Invoke(StoredGenerationInfo[generationKey], StoredResourcesGeneration[generationKey], StoredGenerationInfo[generationKey].GenerationTime);

            ResourceGenerationSave.SaveGeneration(generationKey, newResourceValue);
        }

        private IEnumerator ResourceGeneration(string resourceInfo, float generationTime)
        {
            WaitForSeconds generationDelay = new WaitForSeconds(generationTime);

            while (true)
            {
                yield return generationDelay;

                int resourceValue = StoredResourcesGeneration[resourceInfo];
                resourcesManager.ChangeResource(resourceInfo, resourceValue, ActionType.Add);
            }
        }
    }
}
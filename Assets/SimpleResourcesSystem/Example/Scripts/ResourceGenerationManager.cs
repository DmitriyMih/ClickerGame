using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleResourcesSystem.Example
{
    [RequireComponent(typeof(ResourcesManager))]
    public class ResourceGenerationManager : MonoBehaviour
    {
        private ResourcesManager resourcesManager;
        public Dictionary<ResourceType, int> ResourcesGenerationInfo { get; private set; } = new();
        private Dictionary<ResourceType, Coroutine> coroutineDictionary = new();

        private void Awake()
        {            
            resourcesManager = GetComponent<ResourcesManager>();
        }

        private void Start()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                ResourceItemInfo resourceInfo = resourcesManager.StoredResourcesInfo.ElementAt(i).Value;
                ResourcesGenerationInfo.Add(resourceInfo.ResourcesType, ResourceGenerationSave.LoadGenerationValue(resourceInfo.ResourcesType, 1));
                SetGenerationState(resourceInfo.ResourcesType, ResourcesGenerationInfo[resourceInfo.ResourcesType] > 0, resourceInfo.GenerationTime);
            }
        }

        private void SetGenerationState(ResourceType resourceType, bool state, float generationTime)
        {
            if (coroutineDictionary.ContainsKey(resourceType))
                StopCoroutine(coroutineDictionary[resourceType]);

            if (!state)
                return;

            Coroutine coroutine = StartCoroutine(ResourceGeneration(resourceType, generationTime));
            coroutineDictionary.Add(resourceType, coroutine);
        }

        public void AddGeneratedResourceValue(ResourceType resourceType, int value)
        {
            if (!ResourcesGenerationInfo.ContainsKey(resourceType))
                return;

            value = Mathf.Clamp(value, 0, int.MaxValue);
            ResourcesGenerationInfo[resourceType] = Mathf.Clamp(ResourcesGenerationInfo[resourceType] + value, 0, int.MaxValue);
        }

        private IEnumerator ResourceGeneration(ResourceType resourceType, float generationTime)
        {
            WaitForSeconds wait = new WaitForSeconds(generationTime);

            while (true)
            {
                yield return wait;

                int resourceValue = ResourcesGenerationInfo[resourceType];
                resourcesManager.SetResource(resourceType, resourceValue, ActionType.Add);
            }
        }
    }
}
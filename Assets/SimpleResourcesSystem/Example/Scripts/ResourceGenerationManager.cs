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
        public Dictionary<string, int> ResourcesGenerationInfo { get; private set; } = new();
        private Dictionary<string, Coroutine> coroutineDictionary = new();

        private void Awake()
        {
            resourcesManager = GetComponent<ResourcesManager>();
        }

        private void Start()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                if (resourcesManager.StoredResourcesInfo.ElementAt(i).Value is ResourceGenerationInfo resourceInfo)
                {
                    ResourcesGenerationInfo.Add(resourceInfo.ResourcesKey, ResourceGenerationSave.LoadGenerationValue(resourceInfo.ResourcesKey, 1));
                    SetGenerationState(resourceInfo.ResourcesKey, ResourcesGenerationInfo[resourceInfo.ResourcesKey] > 0, resourceInfo.GenerationTime);
                }
            }
        }

        private void SetGenerationState(string resourceKey, bool state, float generationTime)
        {
            if (coroutineDictionary.ContainsKey(resourceKey))
                StopCoroutine(coroutineDictionary[resourceKey]);

            if (!state)
                return;

            Coroutine coroutine = StartCoroutine(ResourceGeneration(resourceKey, generationTime));
            coroutineDictionary.Add(resourceKey, coroutine);
        }

        public void AddGeneratedResourceValue(string resourceInfo, int value)
        {
            if (!ResourcesGenerationInfo.ContainsKey(resourceInfo))
                return;

            value = Mathf.Clamp(value, 0, int.MaxValue);
            ResourcesGenerationInfo[resourceInfo] = Mathf.Clamp(ResourcesGenerationInfo[resourceInfo] + value, 0, int.MaxValue);
        }

        private IEnumerator ResourceGeneration(string resourceInfo, float generationTime)
        {
            WaitForSeconds wait = new WaitForSeconds(generationTime);

            while (true)
            {
                yield return wait;

                int resourceValue = ResourcesGenerationInfo[resourceInfo];
                resourcesManager.SetResource(resourceInfo, resourceValue, ActionType.Add);
            }
        }
    }
}
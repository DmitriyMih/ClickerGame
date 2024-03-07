using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSystem.Resources
{
    [RequireComponent(typeof(ResourcesManager))]
    public class ResourceGenerationManager : Singleton<ResourceGenerationManager>
    {
        private ResourcesManager resourcesManager;
        public Dictionary<ResourceType, int> ResourcesGenerationInfo { get; private set; } = new();
        private Dictionary<ResourceType, Coroutine> coroutineDictionary = new();

        protected override void Awake()
        {
            base.Awake();
            resourcesManager = GetComponent<ResourcesManager>();
        }

        private void Start()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                ResourceItemInfo resourceInfo = resourcesManager.StoredResourcesInfo.ElementAt(i).Value;
                ResourcesGenerationInfo.Add(resourceInfo.ResourcesType, 1);

                Coroutine coroutine = StartCoroutine(ResourceGeneration(resourceInfo.ResourcesType, resourceInfo.GenerationTime));
                coroutineDictionary.Add(resourceInfo.ResourcesType, coroutine);
            }
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
                resourcesManager.AddResource(resourceType, resourceValue);
            }
        }
    }
}
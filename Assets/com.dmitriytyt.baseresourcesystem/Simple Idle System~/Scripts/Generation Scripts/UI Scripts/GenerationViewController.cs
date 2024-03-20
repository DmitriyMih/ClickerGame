using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.IdleSystem.GenerationSystem
{
    public class GenerationViewController : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup group;

        [Space]
        [SerializeField] private ResourcesIdleManager resourcesManager;
        [SerializeField] private ResourcesGenerationManager generationManager;
        [SerializeField] private GenerationViewItem generationViewPrefab;

        private Dictionary<string, GenerationViewItem> generationsView = new();

        private void Awake()
        {
            Assert.IsNotNull(group);
            Assert.IsNotNull(generationManager);
            Assert.IsNotNull(resourcesManager);
            Assert.IsNotNull(generationViewPrefab);

            resourcesManager.OnResourcesChanged += OnResourceChanged;
            generationManager.OnGenerationChanged += OnGenerationChanged;
        }

        private void Start()
        {
            Inititalization();
        }

        private void OnDestroy()
        {
            resourcesManager.OnResourcesChanged -= OnResourceChanged;
            generationManager.OnGenerationChanged -= OnGenerationChanged;
        }

        private void Inititalization()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                if (resourcesManager.StoredResourcesInfo.ElementAt(i).Value is ResourceGenerationInfo generationInfo)
                {
                    GenerationViewItem generationViewItem = Instantiate(generationViewPrefab, group.transform);
                    generationsView.Add(generationInfo.ResourcesKey, generationViewItem);

                    generationViewItem.Inititalization(generationInfo.ResourceSprite, generationInfo.ResourcesKey, resourcesManager.StoredResources[generationInfo.ResourcesKey]);
                    generationViewItem.SetGenerationAmountValue(generationManager.StoredResourcesGeneration[generationInfo.ResourcesKey], generationManager.StoredGenerationInfo[generationInfo.ResourcesKey].GenerationTime);
                }
            }
        }

        private void OnGenerationChanged(ResourceGenerationInfo resourceInfo, int value, float time)
        {
            if (!generationsView.ContainsKey(resourceInfo.ResourcesKey)) return;
            generationsView[resourceInfo.ResourcesKey].SetGenerationAmountValue(value, time);
        }

        private void OnResourceChanged(ResourceInfo resourceInfo, int value)
        {
            if (!generationsView.ContainsKey(resourceInfo.ResourcesKey)) return;
            generationsView[resourceInfo.ResourcesKey].SetAmountValue(value);
        }
    }
}
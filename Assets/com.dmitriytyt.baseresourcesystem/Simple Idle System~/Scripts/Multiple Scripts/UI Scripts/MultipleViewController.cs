using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.IdleSystem.GenerationSystem
{
    public class MultipleViewController : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup group;

        [Space]
        [SerializeField] private ResourcesIdleManager resourcesManager;
        [SerializeField] private ResourcesViewItem resourceViewPrefab;

        [Space]
        [SerializeField] private ResourcesGenerationManager generationManager;
        [SerializeField] private GenerationViewItem generationViewPrefab;

        private Dictionary<string, ResourcesViewItem> revourcesView = new();
        private Dictionary<string, GenerationViewItem> generationView = new();

        private void Awake()
        {
            Assert.IsNotNull(group);
            Assert.IsNotNull(resourcesManager);
            Assert.IsNotNull(resourceViewPrefab);

            resourcesManager.OnResourcesChanged += OnResourceChanged;
            if (generationManager != null)
            {
                Assert.IsNotNull(generationViewPrefab);
                generationManager.OnGenerationChanged += OnGenerationChanged;
            }
        }

        private void Start()
        {
            Inititalization();
        }

        private void OnDestroy()
        {
            resourcesManager.OnResourcesChanged -= OnResourceChanged;
            if (generationManager != null)
                generationManager.OnGenerationChanged -= OnGenerationChanged;
        }

        private void Inititalization()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                ResourceInfo resourceInfo = resourcesManager.StoredResourcesInfo.ElementAt(i).Value;
                ResourcesViewItem viewItem;

                if (resourcesManager.StoredResourcesInfo.ElementAt(i).Value is ResourceGenerationInfo generationInfo)
                {
                    viewItem = Instantiate(generationViewPrefab, group.transform);
                    generationView.Add(resourceInfo.ResourcesKey, (GenerationViewItem)viewItem);
                }
                else
                    viewItem = Instantiate(resourceViewPrefab, group.transform);

                revourcesView.Add(resourceInfo.ResourcesKey, viewItem);
                viewItem.Inititalization(resourceInfo.ResourceSprite, resourceInfo.ResourcesKey.ToString(), resourcesManager.StoredResources[resourceInfo.ResourcesKey]);
            }
        }

        private void OnGenerationChanged(ResourceGenerationInfo generationInfo, int value, float time)
        {
            if (!generationView.ContainsKey(generationInfo.ResourcesKey)) return;
            generationView[generationInfo.ResourcesKey].SetGenerationAmountValue(value, time);
        }

        private void OnResourceChanged(ResourceInfo resourceInfo, int value)
        {
            if (!revourcesView.ContainsKey(resourceInfo.ResourcesKey)) return;
            revourcesView[resourceInfo.ResourcesKey].SetAmountValue(value);
        }
    }
}
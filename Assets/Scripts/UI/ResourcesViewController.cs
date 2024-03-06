using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace GameSystem.Resources
{
    public class ResourcesViewController : MonoBehaviour
    {
        [SerializeField] private ResourcesManager resourcesManager;

        [SerializeField] private GridLayoutGroup group;
        [SerializeField] private ResourcesViewItem viewPrefab;

        private Dictionary<ResourceType, ResourcesViewItem> revourcesView = new();

        private void Awake()
        {
            Assert.IsNotNull(group);
            Assert.IsNotNull(viewPrefab);
            Assert.IsNotNull(resourcesManager);
        }

        private void Start()
        {
            Inititalization();

            resourcesManager.OnResourcesChanged += OnResourceChanged;
        }

        private void OnDestroy()
        {
            resourcesManager.OnResourcesChanged -= OnResourceChanged;
        }

        private void Inititalization()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                ResourceItemInfo resourceInfo = resourcesManager.StoredResourcesInfo.ElementAt(i).Value;
                ResourceType resourceType = resourceInfo.ResourcesType;

                ResourcesViewItem viewItem = Instantiate(viewPrefab, group.transform);
                viewItem.Inititalization(resourceInfo.ResourceSprite, resourceType.ToString(), resourcesManager.StoredResources[resourceType]);
             
                revourcesView.Add(resourceType, viewItem);
            }
        }

        private void OnResourceChanged(ResourceType resourceType, int value)
        {
            if (!revourcesView.ContainsKey(resourceType)) return;
            revourcesView[resourceType].SetAmountValue(value);
        }
    }
}
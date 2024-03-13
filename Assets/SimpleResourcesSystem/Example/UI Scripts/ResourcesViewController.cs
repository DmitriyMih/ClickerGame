using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.Example
{
    public class ResourcesViewController : MonoBehaviour
    {
        [SerializeField] private ResourcesManager resourcesManager;

        [SerializeField] private GridLayoutGroup group;
        [SerializeField] private ResourcesViewItem viewPrefab;

        private Dictionary<string, ResourcesViewItem> revourcesView = new();

        private void Awake()
        {
            Assert.IsNotNull(group);
            Assert.IsNotNull(viewPrefab);
            Assert.IsNotNull(resourcesManager);

            resourcesManager.OnResourcesChanged += OnResourceChanged;
        }

        private void Start()
        {
            Inititalization();

        }

        private void OnDestroy()
        {
            resourcesManager.OnResourcesChanged -= OnResourceChanged;
        }

        private void Inititalization()
        {
            for (int i = 0; i < resourcesManager.StoredResourcesInfo.Count; i++)
            {
                ResourceInfo resourceInfo = resourcesManager.StoredResourcesInfo.ElementAt(i).Value;

                ResourcesViewItem viewItem = Instantiate(viewPrefab, group.transform);
                viewItem.Inititalization(resourceInfo.ResourceSprite, resourceInfo.ResourcesKey.ToString(), resourcesManager.StoredResources[resourceInfo.ResourcesKey]);

                revourcesView.Add(resourceInfo.ResourcesKey, viewItem);
            }
        }

        private void OnResourceChanged(ResourceInfo resourceInfo, int value)
        {
            if (!revourcesView.ContainsKey(resourceInfo.ResourcesKey)) return;
            revourcesView[resourceInfo.ResourcesKey].SetAmountValue(value);
        }
    }
}
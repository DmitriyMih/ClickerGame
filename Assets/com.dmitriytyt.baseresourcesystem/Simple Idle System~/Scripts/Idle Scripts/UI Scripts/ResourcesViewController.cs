using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.IdleSystem
{
    public class ResourcesViewController : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup group;

        [Space]
        [SerializeField] private ResourcesIdleManager resourcesManager;
        [SerializeField] private ResourcesViewItem resourceViewPrefab;

        private Dictionary<string, ResourcesViewItem> revourcesView = new();

        private void Awake()
        {
            Assert.IsNotNull(group);
            Assert.IsNotNull(resourcesManager);
            Assert.IsNotNull(resourceViewPrefab);

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
                ResourcesViewItem viewItem = Instantiate(resourceViewPrefab, group.transform);

                revourcesView.Add(resourceInfo.ResourcesKey, viewItem);
                viewItem.Inititalization(resourceInfo.ResourceSprite, resourceInfo.ResourcesKey.ToString(), resourcesManager.StoredResources[resourceInfo.ResourcesKey]);
            }
        }

        private void OnResourceChanged(ResourceInfo resourceInfo, int value)
        {
            if (!revourcesView.ContainsKey(resourceInfo.ResourcesKey)) return;
            revourcesView[resourceInfo.ResourcesKey].SetAmountValue(value);
        }
    }
}
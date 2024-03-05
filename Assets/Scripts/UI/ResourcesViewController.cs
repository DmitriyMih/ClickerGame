using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.Resources
{
    public class ResourcesViewController : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup group;
        [SerializeField] private ResourcesViewItem viewPrefab;

        private Dictionary<ResourceType, ResourcesViewItem> revourcesView = new();

        public void Inititalization(ResourcesManager manager, Dictionary<ResourceItemInfo, int> resourcesInfo)
        {
            if (manager == null) return;

            if (viewPrefab == null)
            {
                Debug.LogError($"View Prefab {gameObject} Is Null");
                return;
            }

            for (int i = 0; i < resourcesInfo.Count; i++)
            {
                ResourceType resourceType = resourcesInfo.ElementAt(i).Key.ResourcesType;
                if (revourcesView.ContainsKey(resourceType))
                {
                    Debug.LogError($"Resources {resourceType} Is Already Initialized In Dictionary");
                    continue;
                }

                ResourcesViewItem viewItem = Instantiate(viewPrefab, group.transform);
                viewItem.Inititalization(resourcesInfo.ElementAt(i).Key.ResourceSprite, resourceType.ToString(), resourcesInfo.ElementAt(i).Value);
                revourcesView.Add(resourceType, viewItem);
            }

            manager.OnResourcesChanged += OnResourceChanged;
        }

        private void OnResourceChanged(ResourceType resourceType, int value)
        {
            //Debug.Log($"Resource Changed {resourceType} | {value}");
            
            if (!revourcesView.ContainsKey(resourceType)) return;
            revourcesView[resourceType].SetAmountValue(value);
        }
    }
}
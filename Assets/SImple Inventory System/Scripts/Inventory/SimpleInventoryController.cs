using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleResourcesSystem.SimpleInventorySystem
{
    using SimpleItemSystem;

    public class SimpleInventoryController : MonoBehaviour
    {
        [SerializeField] public string inventorySaveKey = "InventorySaveKey";

        private const int resourcesCount = 25;
        private Dictionary<int, BaseInventoryItemInfo> storedResources = new();

        private void Awake()
        {
            InititalizationInventory();
        }

        [ContextMenu("Load Resources Test")]
        private void Test()
        {
            Debug.Log($"Load {SimpleResourcesDatabase.ResourcesInfo.Count}");        
        }

        private void InititalizationInventory()
        {
            for (int i = 0; i < resourcesCount; i++)
            {

            }
        }
    }
}
using System;
using UnityEngine;

namespace SimpleResourcesSystem.SimpleInventorySystem
{
    using SimpleItemSystem;

    public class BaseInventoryItemInfo
    {
        [field: SerializeField] public SimpleResourcesItemInfo ItemInfo { get; private set; }
        [field: SerializeField] public int ItemAmount { get; private set; }

        public event Action<SimpleResourcesItemInfo> OnItemChanged;
        public event Action<int> OnItemAmountChanged;

        public bool HasItem() => ItemInfo != null;

        public virtual void AddItem(SimpleResourcesItemInfo newItemInfo, int newItemAmount)
        {
            ItemInfo = newItemInfo;
            ItemAmount = newItemAmount;
        }

        public virtual void RemoveItem()
        {
            ItemInfo = null;
            ItemAmount = 0;
        }
    }
}
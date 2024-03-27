using System;
using UnityEngine;

namespace SimpleResourcesSystem.SimpleInventorySystem
{
    using SimpleItemSystem;

    public class BaseInventoryItemInfo
    {
        private SimpleResourcesItemInfo itemInfo;
        private int itemAmount;

        [field: SerializeField]
        public SimpleResourcesItemInfo ItemInfo
        {
            get => itemInfo;
            private set
            {
                itemInfo = value;
                OnItemChanged?.Invoke(itemInfo);
            }
        }

        [field: SerializeField]
        public int ItemAmount
        {
            get => itemAmount;
            private set
            {
                if (itemAmount != value)
                {
                    itemAmount = Mathf.Clamp(value, 0, int.MaxValue);
                    OnItemAmountChanged?.Invoke(itemAmount);
                }
            }
        }

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
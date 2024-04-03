using UnityEngine;

namespace SimpleResourcesSystem.SimpleItemSystem
{
    [CreateAssetMenu(menuName = "Simple Resources/New Simple Recource Info")]
    public class SimpleResourcesItemInfo : BaseResourceInfo
    {
        public SimpleResourcesItemInfo() { }

        [LoadConstructorMarker(0, 1)]
        public SimpleResourcesItemInfo(string name, string resourcesKey)
        {
            this.name = name;
            Name = name;
            ResourcesKey = resourcesKey;
        }

        [field: Header("Item Info")]
        [field: SerializeField, LoadMarker(3)] public string[] Keywords { get; private set; } = new string[] { "SimpleResource" };
        [field: SerializeField, LoadMarker(4)] public string Description { get; private set; } = "Simple Resource Description";
        [field: SerializeField, LoadMarker(2)] public int MaximumResourcesCount { get; private set; } = 64;
    }
}
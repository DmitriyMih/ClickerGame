using UnityEngine;

namespace SimpleResourcesSystem.SimpleItemSystem
{
    [CreateAssetMenu(menuName = "Simple Resources/New Simple Recource Info")]
    public class SimpleResourcesItemInfo : BaseResourceInfo
    {
        public SimpleResourcesItemInfo() { }

        [LoadConstructorMarker(1)] public SimpleResourcesItemInfo(string resourcesKey) => ResourcesKey = resourcesKey;
        [LoadConstructorMarker(0, 2)] public SimpleResourcesItemInfo(string resourcesKey, int testY) => ResourcesKey = resourcesKey;

        [field: Header("Item Info")]
        [field: SerializeField, LoadMarker(3)] public string[] Keywords { get; private set; } = new string[] { "SimpleResource" };
        [field: SerializeField, LoadMarker(4)] public string Description { get; private set; } = "Simple Resource Description";
        [field: SerializeField, LoadMarker(2)] public int MaximumResourcesCount { get; private set; } = 64;
        [field: SerializeField, LoadMarker(5)] public int MaximumResourcesCount5 { get; private set; } = 1;
        [field: SerializeField, LoadMarker(6)] public int MaximumResourcesCount6 { get; private set; } = 16;
        [field: SerializeField, LoadMarker(7)] public int MaximumResourcesCount7 { get; private set; } = 5;
        [field: SerializeField, LoadMarker(8)] public int MaximumResourcesCount8 { get; private set; } = 10;
    }
}
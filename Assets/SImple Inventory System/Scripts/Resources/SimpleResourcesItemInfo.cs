using UnityEngine;

namespace SimpleResourcesSystem.SimpleItemSystem
{
    [CreateAssetMenu(menuName = "Simple Resources/New Simple Recource Info")]
    public class SimpleResourcesItemInfo : BaseResourceInfo
    {
        [field: SerializeField] public int MaximumResourcesCount { get; private set; } = 64;

        [field: Header("Item Info")]
        [field: SerializeField] public string[] Keywords { get; private set; } = new string[] { "SimpleResource" };
        [field: SerializeField] public string Description { get; private set; } = "Simple Resource Description";
    }
}
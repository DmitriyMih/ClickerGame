using UnityEngine;

namespace SimpleResourcesSystem
{
    [CreateAssetMenu(menuName = "Resources/New Recource Info")]
    public class ResourceInfo : ScriptableObject
    {
        [field: SerializeField] public string ResourcesKey { get; private set; }
        [field: SerializeField] public Sprite ResourceSprite { get; private set; }
    }
}
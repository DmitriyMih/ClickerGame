using UnityEngine;

namespace SimpleResourcesSystem
{
    [CreateAssetMenu(menuName = "Resources/New Recource")]
    public class ResourceItemInfo : ScriptableObject
    {
        [field: SerializeField] public ResourceType ResourcesType { get; private set; }

        [field: Header("Item View")]
        [field: SerializeField] public Sprite ResourceSprite;

        [field: Header("Generation Settings")]
        [field: SerializeField] public float GenerationTime = 1f;
    } 
}
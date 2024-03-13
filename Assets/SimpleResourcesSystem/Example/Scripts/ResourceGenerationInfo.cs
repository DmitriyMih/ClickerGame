using UnityEngine;

namespace SimpleResourcesSystem.Example
{
    [CreateAssetMenu(menuName = "Resources/New Generation Info")]
    public class ResourceGenerationInfo : ResourceInfo
    {
        [field: SerializeField] public float GenerationTime { get; private set; } = 1f;
    }
}
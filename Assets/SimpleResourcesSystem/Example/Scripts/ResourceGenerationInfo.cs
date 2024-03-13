using UnityEngine;

namespace SimpleResourcesSystem.Example
{
    [CreateAssetMenu(menuName = "Resources/New Generation Info")]
    public class ResourceGenerationInfo : ResourceInfo
    {
        //[field: SerializeField] public ResourceInfo ResourceInfo { get; private set; }
        [field: SerializeField] public float GenerationTime { get; private set; } = 1f;
    }
}
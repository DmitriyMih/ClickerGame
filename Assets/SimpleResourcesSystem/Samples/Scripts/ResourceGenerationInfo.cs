using UnityEngine;

namespace SimpleResourcesSystem.DemoSamples
{
    [CreateAssetMenu(menuName = "Resources/New Generation Info")]
    public class ResourceGenerationInfo : ResourceInfo
    {
        [field: SerializeField] public float GenerationTime { get; private set; } = 1f;
    }
}
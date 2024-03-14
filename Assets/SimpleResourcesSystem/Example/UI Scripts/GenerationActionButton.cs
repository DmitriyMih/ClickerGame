using UnityEngine;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.Example
{
    public class GenerationActionButton : BaseActionButton
    {
        [Header("Generation Settings")]
        [SerializeField] private ResourcesGenerationManager generationManager;
        [SerializeField] private ResourceGenerationInfo generationInfo;
        [SerializeField] private int generationValue;

        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(generationManager);
            Assert.IsNotNull(generationInfo);

            actionTitle.text = $"{actionType} {generationInfo.ResourcesKey}" +
                $"\nGeneration";
        }

        protected override void OnActionButtonClick()
        {
            generationManager.SetResourceGeneration(generationInfo.ResourcesKey, generationValue, actionType);
        }
    }
}
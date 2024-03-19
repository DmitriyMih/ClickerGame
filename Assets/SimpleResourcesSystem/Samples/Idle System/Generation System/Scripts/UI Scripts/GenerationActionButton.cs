using UnityEngine;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.IdleSystem.GenerationSystem
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

            actionTitle.text = $"{actionType} {generationValue} {generationInfo.ResourcesKey}" +
                $"\nGeneration";
        }

        protected override void OnActionButtonClick()
        {
            generationManager.SetResourceGeneration(generationInfo.ResourcesKey, generationValue, actionType);
        }
    }
}
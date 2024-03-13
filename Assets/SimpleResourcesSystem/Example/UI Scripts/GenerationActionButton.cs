using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.Example
{
    [RequireComponent(typeof(Button))]
    public class GenerationActionButton:MonoBehaviour
    {
        private Button actionButton;
        [SerializeField] private ResourceGenerationManager generationManager;

        [Space]
        [SerializeField] private ActionType actionType;

        [SerializeField] private ResourceGenerationInfo generationInfo;
        [SerializeField] private int generationValue;

        private void Awake()
        {
            Assert.IsNotNull(generationManager);
            Assert.IsNotNull(generationInfo);

            actionButton = GetComponent<Button>();
            actionButton.onClick.AddListener(() => OnActionButtonClick());
        }

        private void OnActionButtonClick()
        {
            generationManager.SetResourceGeneration(generationInfo.ResourcesKey, generationValue, actionType);
        }
    }
}
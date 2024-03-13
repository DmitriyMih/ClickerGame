using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.Example
{
    [RequireComponent(typeof(Button))]
    public class ResourcesActionButton : MonoBehaviour
    {
        private Button actionButton;
        [SerializeField] private ResourcesManager resourcesManager;

        [Space]
        [SerializeField] private ActionType actionType;

        [SerializeField] private ResourceInfo resourcesInfo;
        [SerializeField] private int resourceValue;

        private void Awake()
        {
            Assert.IsNotNull(resourcesManager);
            Assert.IsNotNull(resourcesInfo);

            actionButton = GetComponent<Button>();
            actionButton.onClick.AddListener(() => OnActionButtonClick());
        }

        private void OnActionButtonClick()
        {
            resourcesManager.SetResource(resourcesInfo.ResourcesKey, resourceValue, actionType);
        }
    }
}
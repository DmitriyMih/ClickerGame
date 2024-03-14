using UnityEngine;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.Example
{
    public class ResourcesActionButton : BaseActionButton
    {
        [Header("Resources Settings")]
        [SerializeField] private ResourcesManager resourcesManager;

        [SerializeField] private ResourceInfo resourcesInfo;
        [SerializeField] private int resourceValue;

        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(resourcesManager);
            Assert.IsNotNull(resourcesInfo);

            actionTitle.text = $"{actionType}" +
                $"\n{resourcesInfo.ResourcesKey}";
        }

        protected override void OnActionButtonClick()
        {
            resourcesManager.SetResource(resourcesInfo.ResourcesKey, resourceValue, actionType);
        }
    }
}
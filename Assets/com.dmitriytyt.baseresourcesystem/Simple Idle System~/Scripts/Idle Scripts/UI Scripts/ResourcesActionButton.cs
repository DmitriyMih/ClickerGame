using UnityEngine;
using UnityEngine.Assertions;

namespace SimpleResourcesSystem.IdleSystem
{
    public class ResourcesActionButton : BaseActionButton
    {
        [Header("Resources Settings")]
        [SerializeField] private ResourcesIdleManager resourcesManager;

        [SerializeField] private ResourceInfo resourcesInfo;
        [SerializeField] private int resourceValue;

        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(resourcesManager);
            Assert.IsNotNull(resourcesInfo);

            actionTitle.text = $"{actionType} {resourceValue}" +
                $"\n{resourcesInfo.ResourcesKey}";
        }

        protected override void OnActionButtonClick()
        {
            resourcesManager.ChangeResource(resourcesInfo.ResourcesKey, resourceValue, actionType);
        }
    }
}
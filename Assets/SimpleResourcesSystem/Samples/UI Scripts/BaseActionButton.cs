using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

namespace SimpleResourcesSystem.DemoSamples
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseActionButton : MonoBehaviour
    {
        protected Button actionButton;
        [SerializeField] protected TextMeshProUGUI actionTitle;
        
        [Space]
        [SerializeField] protected ActionType actionType;

        protected virtual void Awake()
        {
            Assert.IsNotNull(actionTitle);

            actionButton = GetComponent<Button>();
            actionButton.onClick.AddListener(() => OnActionButtonClick());
        }

        protected abstract void OnActionButtonClick();
    }
}
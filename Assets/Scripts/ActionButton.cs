using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSystem.Resources;

namespace GameSystem.Resources.Example
{
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        private Button actionButton;

        [SerializeField] private ResourceType resourcesType;
        [SerializeField] private int resourceValue;

        private void Awake()
        {
            actionButton = GetComponent<Button>();
            actionButton.onClick.AddListener(() => OnActionButtonClick());
        }

        private void OnActionButtonClick()
        {
            if (ResourcesManager.Instance != null)
                ResourcesManager.Instance.AddResource(resourcesType, resourceValue);
        }
    }
}
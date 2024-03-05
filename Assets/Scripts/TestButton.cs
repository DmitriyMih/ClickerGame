using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSystem.Resources;

[RequireComponent(typeof(Button))]
public class TestButton : MonoBehaviour
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
        ResourcesManager.AddResource(resourcesType, resourceValue);
    }
}
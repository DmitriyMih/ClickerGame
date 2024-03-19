using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

namespace SimpleResourcesSystem.IdleSystem
{
    public class ResourcesViewItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resourceAmountText;
        [SerializeField] private TextMeshProUGUI resourceNameText;

        [SerializeField] private Image resourceIcon;

        protected virtual void Awake()
        {
            Assert.IsNotNull(resourceIcon);

            Assert.IsNotNull(resourceNameText);
            Assert.IsNotNull(resourceAmountText);
        }

        public void Inititalization(Sprite resourceSprite, string resourceName, int resourceValue)
        {
            resourceIcon.sprite = resourceSprite;
            resourceNameText.text = resourceName;

            SetAmountValue(resourceValue);
        }

        public void SetAmountValue(int value)
        {
            resourceAmountText.text = value.ToString();
        }
    }
}
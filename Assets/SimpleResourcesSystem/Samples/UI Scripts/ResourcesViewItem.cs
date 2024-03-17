using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

namespace SimpleResourcesSystem.DemoSamples
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

        public void Inititalization(Sprite recourceSprite, string recourceName, int value)
        {
            resourceIcon.sprite = recourceSprite;
            resourceNameText.text = recourceName;

            SetAmountValue(value);
        }

        public void SetAmountValue(int value)
        {
            resourceAmountText.text = value.ToString();
        }
    }
}
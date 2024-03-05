using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GameSystem.Resources
{
    public class ResourcesViewItem: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI recourceAmountText;
        [SerializeField] private TextMeshProUGUI recourceNameText;

        [SerializeField] private Image resourceIcon;

        public void Inititalization(Sprite recourceSprite, string recourceName, int value)
        {
            if (resourceIcon !=null)
                resourceIcon.sprite = recourceSprite;

            if (recourceNameText != null)
                recourceNameText.text = recourceName;

            SetAmountValue(value);
        }

        public void SetAmountValue(int value)
        {
            if(recourceAmountText==null)
            {
                Debug.LogError($"Recource Text {gameObject} | Is Null");
                return;
            }

            recourceAmountText.text = value.ToString();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

namespace SimpleResourcesSystem.IdleSystem.GenerationSystem
{
    public class GenerationViewItem : ResourcesViewItem
    {
        [SerializeField] private TextMeshProUGUI generationAmountText;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(generationAmountText);
        }

        public void Inititalization(Sprite recourceSprite, string recourceName, int resourceValue, int generationValue, float generationTime)
        {
            Inititalization(recourceSprite, recourceName, resourceValue);
            SetGenerationAmountValue(generationValue, generationTime);
        }

        public void SetGenerationAmountValue(int value, float time)
        {
            generationAmountText.text = $"+{value}/{Mathf.RoundToInt(time)} sec";
        }
    }
}
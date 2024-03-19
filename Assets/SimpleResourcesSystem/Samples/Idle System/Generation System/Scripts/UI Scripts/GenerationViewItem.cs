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

        public void SetGenerationAmountValue(int value, float time)
        {
            generationAmountText.text = $"+{value}/{Mathf.RoundToInt(time)} sec";
        }
    }
}
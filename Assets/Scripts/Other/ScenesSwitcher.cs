using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

namespace GameSystem.SceneSystem
{
    public class ScenesSwitcher : Singleton<ScenesSwitcher>
    {
        [SerializeField] private Button leftActionButton;
        [SerializeField] private Button rightActionButton;

        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(leftActionButton);
            Assert.IsNotNull(rightActionButton);

            leftActionButton.onClick.AddListener(() => OnActionClick(true));
            leftActionButton.onClick.AddListener(() => OnActionClick(false));
        }

        private void OnActionClick(bool isLeft)
        {
            //int currentScene = SceneManager.GetActiveScene().buildIndex;
            //SceneManager.LoadScene(nextIndex);
        }
    }
}
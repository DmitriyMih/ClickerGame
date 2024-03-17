using UnityEngine;

namespace SimpleResourcesSystem.DemoSamples
{
    internal class ResourceGenerationSave
    {
        private const string generationKey = "GenerationAmount";

        public static int LoadGenerationValue(string resourceKey, int defaultValue = 0)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return PlayerPrefs.GetInt($"{resourceKey}{sceneIndex}{generationKey}", defaultValue);
        }

        public static void SaveGeneration(string resourceKey, int resourceValue)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            PlayerPrefs.SetInt($"{resourceKey}{sceneIndex}{generationKey}", resourceValue);
            PlayerPrefs.Save();
        }
    }
}
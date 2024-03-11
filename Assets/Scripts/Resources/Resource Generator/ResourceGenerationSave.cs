using UnityEngine;

namespace GameSystem.Resources
{
    internal class ResourceGenerationSave
    {
        private const string generationKey = "GenerationAmount";

        public static int LoadGenerationValue(ResourceType resourcesType, int defaultValue = 0)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return PlayerPrefs.GetInt($"{resourcesType}{sceneIndex}{generationKey}", defaultValue);
        }

        public static void SaveGeneration(ResourceType resourcesType, int resourceValue)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            PlayerPrefs.SetInt($"{resourcesType}{sceneIndex}{generationKey}", resourceValue);
            PlayerPrefs.Save();
        }
    }
}
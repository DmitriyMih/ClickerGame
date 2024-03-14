using UnityEngine;

namespace SimpleResourcesSystem
{
    public static class ResourcesSave
    {
        private const string amountKey = "ResourceAmount";

        public static int LoadResource(string resourceKey)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return PlayerPrefs.GetInt($"{resourceKey}{sceneIndex}{amountKey}", 0);
        }

        public static void SaveResource(string resourceKey, int resourceValue)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            PlayerPrefs.SetInt($"{resourceKey}{sceneIndex}{amountKey}", resourceValue);
            PlayerPrefs.Save();
        }

        public static int LoadResource(string resourceKey, int index)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return PlayerPrefs.GetInt($"{resourceKey}{sceneIndex}{index}{amountKey}", 0);
        }

        public static void SaveResource(string resourceKey, int index, int resourceValue)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            PlayerPrefs.SetInt($"{resourceKey}{sceneIndex}{index}{amountKey}", resourceValue);
            PlayerPrefs.Save();
        }
    }
}
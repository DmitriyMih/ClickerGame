using UnityEngine;

namespace SimpleResourcesSystem
{
    public static class ResourcesSave
    {
        private const string amountKey = "ResourceAmount";

        public static int LoadResource(ResourceType resourcesType)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return PlayerPrefs.GetInt($"{resourcesType}{sceneIndex}{amountKey}", 0);
        }

        public static void SaveResource(ResourceType resourcesType, int resourceValue)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            
            PlayerPrefs.SetInt($"{resourcesType}{sceneIndex}{amountKey}", resourceValue);
            PlayerPrefs.Save();
        }
    }
}
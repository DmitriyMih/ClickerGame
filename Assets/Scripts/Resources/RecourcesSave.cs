using UnityEngine;

namespace GameSystem.Resources
{
    public static class RecourcesSave
    {
        #region Default Save

        public static int LoadResource(ResourceType resourcesType)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return PlayerPrefs.GetInt($"{resourcesType}{sceneIndex}", 0);
        }

        public static void SaveResource(ResourceType resourcesType, int resourceValue)
        {
            int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            
            PlayerPrefs.SetInt($"{resourcesType}{sceneIndex}", resourceValue);
            PlayerPrefs.Save();
        }

        #endregion

        #region Save By Scene Index

        public static int LoadResource(int sceneIndex, ResourceType resourcesType)
        {
            return PlayerPrefs.GetInt($"{resourcesType}{sceneIndex}", 0);
        }

        public static void SaveResource(int sceneIndex, ResourceType resourcesType, int resourceValue)
        {
            PlayerPrefs.SetInt($"{resourcesType}{sceneIndex}", resourceValue);
            PlayerPrefs.Save();
        }

        #endregion
    }
}
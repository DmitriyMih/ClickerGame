using UnityEngine;

namespace SimpleResourcesSystem.IdleSystem.GenerationSystem
{
    internal class ResourceGenerationSave
    {
        private const string generationKey = "GenerationAmount";
        private const int defaultValue = 0;

        private static int Load(string key) => PlayerPrefs.GetInt(key, defaultValue);
        private static void Save(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        private static void ChangeKeyByType(ref string key, SaveLoadType loadType)
        {
            if (loadType == SaveLoadType.SingleScene)
                key += UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        #region Simple Save/Load

        public static int LoadGeneration(string resourceKey, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{generationKey}.{resourceKey}";
            ChangeKeyByType(ref key, loadType);
            return Load(key);
        }
         
        public static void SaveGeneration(string resourceKey, int resourceValue, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{generationKey}.{resourceKey}";
            ChangeKeyByType(ref key, loadType);
            Save(key, resourceValue);
        }

        #endregion 

        #region Multiple Save/Load

        public static int LoadGeneration(string managerKey, string resourceKey, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{generationKey}.{resourceKey}.{managerKey}";
            ChangeKeyByType(ref key, loadType);
            return Load(key);
        }

        public static void SaveGeneration(string managerKey, string resourceKey, int resourceValue, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{generationKey}.{resourceKey}.{managerKey}";
            ChangeKeyByType(ref key, loadType);
            Save(key, resourceValue);
        }

        #endregion
    }
}
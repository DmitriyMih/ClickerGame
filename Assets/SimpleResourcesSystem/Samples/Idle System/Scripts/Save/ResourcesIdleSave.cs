using UnityEngine;

namespace SimpleResourcesSystem.IdleSystem
{
    public static class ResourcesIdleSave
    {
        private const string amountKey = "ResourceAmount";
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

        public static int LoadResource(string resourceKey, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{amountKey}.{resourceKey}";
            ChangeKeyByType(ref key, loadType);
            return Load(key);
        }

        public static void SaveResource(string resourceKey, int resourceValue, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{amountKey}.{resourceKey}";
            ChangeKeyByType(ref key, loadType);
            Save(key, resourceValue);
        }

        #endregion 

        #region Multiple Save/Load

        public static int LoadResource(string managerKey, string resourceKey, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{amountKey}.{resourceKey}.{managerKey}";
            ChangeKeyByType(ref key, loadType);

            Debug.Log($"Load Resource By Key {key}");
            return Load(key);
        }

        public static void SaveResource(string managerKey, string resourceKey, int resourceValue, SaveLoadType loadType = SaveLoadType.SingleScene)
        {
            string key = $"{amountKey}.{resourceKey}.{managerKey}";
            ChangeKeyByType(ref key, loadType);

            Debug.Log($"Save Resource By Key {key}");
            Save(key, resourceValue);
        }

        #endregion
    }
}
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace BaseGoogleSheetLoaderSystem
{
    public class GoogleSheetLoaderWindow : EditorWindow
    {
        const string sheetURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSDazInDHxGwQOGH0udx0Z_N8i9NID-oEo50eRnK6aVY7cGsHY2bxMwHd5tchnq1jO9I8OgqsKmSLt3/pub?gid=0&single=true&output=csv";
        static bool inProgress;

        public static event Action<string> LoadCallback;

        protected GUIStyle GetStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 12)
        {
            return new GUIStyle(GUI.skin.label) { alignment = textAnchor, fontStyle = fontStyle, fontSize = fontSize };
        }

        [MenuItem("My Tools/Base Google Sheet Loader Window")]
        public static void ShowWindow()
        {
            GoogleSheetLoaderWindow window = GetWindow<GoogleSheetLoaderWindow>();
            window.titleContent = new GUIContent("Base Google Sheet Loader Window");

            inProgress = false;
        }

        private void OnGUI()
        {
            GUILayout.Label("Base Google Sheet Loader", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

            GUILayout.Space(10);
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            //sheetURL = EditorGUILayout.TextField("Sheet URL", sheetURL);

            if (!inProgress)
            {
                if (GUILayout.Button("Load Sheet"))
                    Load();
            }
            else
                GUILayout.Label("Load In Progress", GetStyle(TextAnchor.MiddleCenter, fontSize: 12));

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        private static async void Load()
        {
            inProgress = true;

            using var www = UnityWebRequest.Get(sheetURL);
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                LoadCallback?.Invoke(www.downloadHandler.text);
                Debug.Log($"Success: {www.downloadHandler.text}");
            }
            else
                Debug.LogError($"Failed: {www.error}");

            inProgress = false;
        }
    }
}
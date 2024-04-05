using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace GoogleSheetLoaderSystem
{
    public class GoogleSheetLoaderWindow : EditorWindow
    {
        const string headerPath = "Assets/Simple Google Sheet/System/Sprites/LoaderHeader.png";
        const string iconPath = "Assets/Simple Google Sheet/System/Sprites/LogoIcon.png";

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);

        string sheetURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSDazInDHxGwQOGH0udx0Z_N8i9NID-oEo50eRnK6aVY7cGsHY2bxMwHd5tchnq1jO9I8OgqsKmSLt3/pub?gid=0&single=true&output=csv";
        static bool inProgress;

        private Vector2 urlSrollRect = Vector2.zero;

        public static event Action<string> LoadCallback;

        [MenuItem("My Tools/Base Google Sheet Loader Window")]
        public static void ShowWindow()
        {
            GoogleSheetLoaderWindow window = GetWindow<GoogleSheetLoaderWindow>();
            window.titleContent = new GUIContent("Google Sheet Loader Window");

            window.maxSize = new Vector2(400f, 250f);
            window.minSize = window.maxSize;

            inProgress = false;
        }

        private void OnGUI()
        {
            WindowSupports.DrawLogo(LogoTexture, IconTexture);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Space(5);

            DrawSheetUrl();

            GUILayout.Space(10);

            if (!inProgress && GUILayout.Button("Load Sheet"))
                Load(sheetURL);

            if (inProgress && GUILayout.Button("Stop Loading"))
            {

            }

            GUILayout.Space(5);
            GUILayout.EndVertical();

            if (inProgress)
            {
                GUILayout.Space(10);

                GUILayout.BeginVertical("HelpBox");
                GUILayout.Space(5);

                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.black;

                GUILayout.Label("Load In Progress", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 12));

                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;

                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
        }

        private void DrawSheetUrl()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            urlSrollRect = EditorGUILayout.BeginScrollView(urlSrollRect, GUILayout.Height(60f));

            sheetURL = EditorGUILayout.TextField(sheetURL);

            if (!string.IsNullOrWhiteSpace(sheetURL))
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.black;

                GUILayout.Label(sheetURL);

                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private static async void Load(string url)
        {
            inProgress = true;

            using var www = UnityWebRequest.Get(url);
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
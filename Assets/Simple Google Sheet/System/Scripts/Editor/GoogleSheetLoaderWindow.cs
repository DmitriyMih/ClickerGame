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

        private bool isUrlLoad = true;
        string sheetURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSDazInDHxGwQOGH0udx0Z_N8i9NID-oEo50eRnK6aVY7cGsHY2bxMwHd5tchnq1jO9I8OgqsKmSLt3/pub?gid=0&single=true&output=csv";
        static bool isProgress;
        private Vector2 urlSrollRect = Vector2.zero;

        private TextAsset textAsset;

        public static event Action<string> LoadCallback;

        [MenuItem("My Tools/Base Google Sheet Loader Window")]
        public static void ShowWindow()
        {
            GoogleSheetLoaderWindow window = GetWindow<GoogleSheetLoaderWindow>();
            window.titleContent = new GUIContent("Google Sheet Loader Window");

            window.maxSize = new Vector2(400f, 250f);
            window.minSize = window.maxSize;

            isProgress = false;
        }

        private void OnGUI()
        {
            WindowSupports.DrawLogo(LogoTexture, IconTexture);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2.5f);

            DrawToggleLoadType();

            GUILayout.Space(2.5f);

            if (isUrlLoad)
                DrawGoogleSheetLoaderBlock();
            else
                DrawTextAssetBlock();

            GUILayout.Space(2.5f);
            GUILayout.EndVertical();

            DrawProgressBlock();
        }

        private void DrawToggleLoadType()
        {
            if (isProgress)
                return;

            string label = isUrlLoad ? "Offline Load" : "Load By URL";

            if (GUILayout.Button($"Change To: {label}"))
                isUrlLoad = !isUrlLoad;
        }

        private void DrawGoogleSheetLoaderBlock()
        {
            DrawSheetUrl();

            GUILayout.Space(5);

            if (!isProgress && GUILayout.Button("Load Sheet"))
                Load(sheetURL);

            if (isProgress && GUILayout.Button("Stop Loading"))
                isProgress = false;
        }

        private void DrawTextAssetBlock()
        {
            textAsset = EditorGUILayout.ObjectField(textAsset, typeof(TextAsset)) as TextAsset;

            if (GUILayout.Button("Extract Text") && textAsset!=null)
                LoadCallback?.Invoke(textAsset.text);
        }

        private void DrawProgressBlock()
        {
            if (!isProgress)
                return;

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.black;

            GUILayout.Label("Load In Progress", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 12));

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;

            GUILayout.Space(5);
            GUILayout.EndVertical();
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
            isProgress = true;

            using var www = UnityWebRequest.Get(url);
            var operation = www.SendWebRequest();

            while (!operation.isDone || !isProgress)
                await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                LoadCallback?.Invoke(www.downloadHandler.text);
                Debug.Log($"Success: {www.downloadHandler.text}");
            }
            else
                Debug.LogError($"Failed: {www.error}");

            isProgress = false;
        }
    }
}

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace BaseGoogleSheetLoaderSystem
{
    public class GoogleSheetLoaderWindow : EditorWindow
    {
        const string publisherUrl = "https://assetstore.unity.com/publishers/98518?preview=1";

        const string headerPath = "Assets/BaseGoogleSheetLoader/System/LoaderHeader.png";
        const string iconPath = "Assets/BaseGoogleSheetLoader/System/LogoIcon.png";

        Color backgroundColor = new Color(1f, 0.96f, 0.84f, 1f);

        int widthOffcetLeft = 65;
        int widthOffcetRight = 10;

        int headerHeight = 75;
        int heightOffcet = 5;

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);


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

            window.maxSize = new Vector2(400f, 250f);   //120
            window.minSize = window.maxSize;

            inProgress = false;
        }

        private void DrawLogo()
        {
            Rect headerBackground = new Rect(0, 0, Screen.width, Screen.height);

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, backgroundColor);
            tex.Apply();
            GUI.DrawTexture(headerBackground, tex);

            Rect headerRect = new Rect(widthOffcetLeft, heightOffcet, Screen.width - widthOffcetRight - widthOffcetLeft, headerHeight - heightOffcet * 2);
            GUI.DrawTexture(headerRect, LogoTexture, ScaleMode.ScaleToFit);

            Rect rect = new Rect(5, (headerHeight - 60) / 2, 60, 60);

            GUI.DrawTexture(rect, IconTexture);
            if (GUI.Button(rect, "", new GUIStyle()))
                Application.OpenURL(publisherUrl);
        }

        private void OnGUI()
        {
            DrawLogo();

            //GUILayout.Label("Base Google Sheet Loader", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));
            //GUILayout.Space(10);

            GUILayout.Space(85);

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            //sheetURL = EditorGUILayout.TextField("Sheet URL", sheetURL);

            if (!inProgress)
            {
                if (GUILayout.Button("Load Sheet"))
                    Load();
            }
            else
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.black;

                GUILayout.Label("Load In Progress", GetStyle(TextAnchor.MiddleCenter, fontSize: 12));

                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
            }

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
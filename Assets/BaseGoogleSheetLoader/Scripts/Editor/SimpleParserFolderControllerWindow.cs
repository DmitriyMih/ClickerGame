using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public sealed class SimpleParserFolderControllerWindow : EditorWindow
    {
        const string publisherUrl = "https://assetstore.unity.com/publishers/98518?preview=1";

        const string headerPath = "Assets/BaseGoogleSheetLoader/System/FolderControllerHeader.png";
        const string iconPath = "Assets/BaseGoogleSheetLoader/System/LogoIcon.png";

        Color backgroundColor = new Color(1f, 0.96f, 0.84f, 1f);

        int widthOffcetLeft = 65;
        int widthOffcetRight = 10;

        int headerHeight = 75;
        int heightOffcet = 5;

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);

        Dictionary<string, string[]> parseStrings = new();

        private Object targetObject;

        ConstructorsStruct constructorsParsePropperties = new();
        Dictionary<int, FieldsStruct> fieldsParsePropperties = new();

        public SimpleParserFolderControllerWindow() => ReconnectToParser();

        protected GUIStyle GetStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 12)
        {
            return new GUIStyle(GUI.skin.label) { alignment = textAnchor, fontStyle = fontStyle, fontSize = fontSize };
        }

        [MenuItem("My Tools/Simple Parser Folder Controller Window")]
        public static void ShowWindow()
        {
            SimpleParserFolderControllerWindow window = GetWindow<SimpleParserFolderControllerWindow>();
            window.titleContent = new GUIContent("Simple Parser Folder Controller Window");

            window.maxSize = new Vector2(400f, 250f);   //120
            window.minSize = window.maxSize;
        }

        private void ReconnectToParser()
        {

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

            GUILayout.Space(85);

            DisplayReconnect();
        }

        private void DisplayReconnect()
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            if (GUILayout.Button("Reconect To Parser"))
                ReconnectToParser();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
    }
}
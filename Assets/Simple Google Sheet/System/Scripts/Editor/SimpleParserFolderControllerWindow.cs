using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GoogleSheetLoaderSystem
{
    public sealed class SimpleParserFolderControllerWindow : EditorWindow
    {
        const string headerPath = "Assets/Simple Google Sheet/System/Sprites/FolderControllerHeader.png";
        const string iconPath = "Assets/Simple Google Sheet/System/Sprites/LogoIcon.png";

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);

        public SimpleParserFolderControllerWindow() => ReconnectToParser();

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

        private void OnGUI()
        {
            WindowSupports.DrawLogo(LogoTexture, IconTexture);

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
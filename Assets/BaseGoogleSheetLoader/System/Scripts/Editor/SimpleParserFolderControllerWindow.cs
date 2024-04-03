using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GoogleSheetLoaderSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public sealed class SimpleParserFolderControllerWindow : EditorWindow
    {
        const string headerPath = "Assets/BaseGoogleSheetLoader/System/Sprites/FolderControllerHeader.png";
        const string iconPath = "Assets/BaseGoogleSheetLoader/System/Sprites/LogoIcon.png";

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);

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
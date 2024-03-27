using UnityEngine;
using UnityEditor;
using BaseGoogleSheetLoaderSystem;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    public class BaseResourcesConverterWindow : EditorWindow
    {
        protected static string titleContent;
        protected static string callbackText;

        protected GUIStyle GetStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 12)
        {
            return new GUIStyle(GUI.skin.label) { alignment = textAnchor, fontStyle = fontStyle, fontSize = fontSize };
        }

        protected static void OpenWindow<TWindow>(string windowTitleContent) where TWindow : EditorWindow
        {
            TWindow window = GetWindow<TWindow>();
            window.titleContent = new GUIContent(windowTitleContent);
            titleContent = windowTitleContent;

            Reconnect();
        }

        private static void Reconnect()
        {
            GoogleSheetLoaderWindow.LoadCallback -= SimpleResourcesGoogleSheetConverterWindow_LoadCallback;

            callbackText = default;
            GoogleSheetLoaderWindow.LoadCallback += SimpleResourcesGoogleSheetConverterWindow_LoadCallback;

            Debug.Log("Reconnect");
        }

        private static void SimpleResourcesGoogleSheetConverterWindow_LoadCallback(string callback)
        {
            callbackText = callback;
            Debug.Log($"Load Callback {callback}");
        }

        private void OnGUI() => DisplayGUI();

        protected virtual void DisplayGUI()
        {
            GUILayout.Label(titleContent, GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

            GUILayout.Space(10);

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            if (GUILayout.Button("Reconect To Loader"))
                Reconnect();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
    }
}
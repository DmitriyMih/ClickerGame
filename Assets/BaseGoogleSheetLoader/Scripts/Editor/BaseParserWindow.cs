using UnityEngine;
using UnityEditor;
using BaseGoogleSheetLoaderSystem;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    public class BaseParserWindow : EditorWindow
    {
        protected static string titleText;
        protected static string callbackText;

        protected static event System.Action OnReconnect;

        public BaseParserWindow()
        {
            OnReconnect += Reconnect;
            Reconnect();
        }

        private void OnDisable() => OnReconnect -= Reconnect;

        protected GUIStyle GetStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 12)
        {
            return new GUIStyle(GUI.skin.label) { alignment = textAnchor, fontStyle = fontStyle, fontSize = fontSize };
        }

        protected static void OpenWindow<TWindow>(string windowTitleContent, out TWindow outWindow) where TWindow : EditorWindow
        {
            TWindow window = GetWindow<TWindow>();
            outWindow = window;

            window.titleContent = new GUIContent(windowTitleContent);
            titleText = windowTitleContent;

            window.maxSize = new Vector2(450, 600f);
            window.minSize = window.maxSize;

            OnReconnect?.Invoke();
        }

        protected virtual void Reconnect()
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
            GUILayout.Space(5);
            GUILayout.Label(titleText, GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

            GUILayout.Space(5);

            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            if (GUILayout.Button("Reconnect To Loader"))
                Reconnect();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
    }
}
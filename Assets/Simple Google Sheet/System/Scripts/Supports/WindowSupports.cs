using UnityEngine;

namespace GoogleSheetLoaderSystem
{
    public static class WindowSupports
    {
        const string publisherUrl = "https://assetstore.unity.com/publishers/98518?preview=1";

        static Color backgroundColor = new Color(1f, 0.96f, 0.84f, 1f);

        const int widthOffcetLeft = 65;
        const int widthOffcetRight = 10;

        const int headerHeight = 75;
        const int heightOffcet = 5;

        public static void SetColorState(bool state)
        {
            GUI.backgroundColor = state ? Color.white : Color.white;
            GUI.contentColor = state ? Color.black : Color.white;
        }

        public static void DrawLogo(Texture2D logoTexture, Texture2D iconTexture)
        {
            Rect headerBackground = new Rect(0, 0, Screen.width, Screen.height);

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, backgroundColor);
            tex.Apply();
            GUI.DrawTexture(headerBackground, tex);

            Rect headerRect = new Rect(widthOffcetLeft, heightOffcet, Screen.width - widthOffcetRight - widthOffcetLeft, headerHeight - heightOffcet * 2);
            GUI.DrawTexture(headerRect, logoTexture, ScaleMode.ScaleToFit);

            Rect rect = new Rect(5, (headerHeight - 60) / 2, 60, 60);

            GUI.DrawTexture(rect, iconTexture);
            if (GUI.Button(rect, "", new GUIStyle()))
                Application.OpenURL(publisherUrl);

            GUILayout.Space(85);
        }

        public static GUIStyle GetStyle(TextAnchor textAnchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = 12)
        {
            return new GUIStyle(GUI.skin.label) { alignment = textAnchor, fontStyle = fontStyle, fontSize = fontSize };
        }

        public static void DrawToggle(this ref bool toggleValue, string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);

            toggleValue = GUILayout.Toggle(toggleValue, "", GUILayout.Width(15));
            GUILayout.Label(label, WindowSupports.GetStyle(TextAnchor.MiddleLeft, FontStyle.BoldAndItalic));

            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
        }
    }
}
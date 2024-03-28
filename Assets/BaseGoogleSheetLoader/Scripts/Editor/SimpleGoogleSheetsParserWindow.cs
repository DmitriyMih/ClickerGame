using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SimpleResourcesSystem.SimpleItemSystem;
using System.Reflection;
using System.Linq;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public class SimpleGoogleSheetsParserWindow : BaseParserWindow
    {
        const string publisherUrl = "https://assetstore.unity.com/publishers/98518?preview=1";

        const string windowTitle = "Simple Google Sheets Parser";
        const string headerPath = "Assets/BaseGoogleSheetLoader/System/ParserHeader.png";
        const string iconPath = "Assets/BaseGoogleSheetLoader/System/LogoIcon.png";

        Color backgroundColor = new Color(1f, 0.96f, 0.84f, 1f);

        int widthOffcetLeft = 65;
        int widthOffcetRight = 10;

        int headerHeight = 75;
        int heightOffcet = 5;

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);

        Dictionary<string, string[]> parseStrings = new();

        private Vector2 mainScrollPosition = Vector2.zero;

        private Vector2 sheetOutputScrollPosition = Vector2.zero;
        private Vector2 sheetItemsScrollPosition = Vector2.zero;

        private bool isShowSheetOutput = false;
        private bool isShowSheetItems = false;

        private Object tempObject;
        private Object targetObject;

        Dictionary<int, ConstructorsStruct> constructorsParsePropperties;
        Dictionary<int, FieldsStruct> fieldsParsePropperties;

        public SimpleGoogleSheetsParserWindow()
        {
            titleContent = windowTitle;
        }

        [MenuItem("My Tools/Simple Resources Item Info Converter Window")]
        public static void ShowWindow()
        {
            OpenWindow<SimpleGoogleSheetsParserWindow>(windowTitle);
        }

        protected override void DisplayGUI()
        {
            DrawLogo();

            GUILayout.Space(85);

            DisplayReconnect();

            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            Display();

            EditorGUILayout.EndScrollView();
        }

        private void DisplayReconnect()
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            if (GUILayout.Button("Reconect To Loader"))
                Reconnect();

            GUILayout.Space(5);
            GUILayout.EndVertical();
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

        private void Display()
        {
            GUILayout.Space(10);

            #region Propperties Logic

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);

            //GUI.contentColor = Color.black;
            targetObject = EditorGUILayout.ObjectField(targetObject, typeof(ScriptableObject));

            if (targetObject != null && GUILayout.Button("Get Parsing Propperties"))
            {
                GetParsingPropperties(targetObject, out constructorsParsePropperties, out fieldsParsePropperties);
                tempObject = targetObject;
            }

            GUILayout.Space(5);
            GUILayout.EndVertical();

            #endregion

            #region Parsing And Output Logic

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5f);

            if (callbackText != null && callbackText != "")
            {
                DisplaySheetOutput();

                GUILayout.Space(10);

                if (GUILayout.Button("Parse Output To Items"))
                    ParseText();
            }

            if (GUILayout.Button("Clear Parse Strings"))
                parseStrings.Clear();

            GUILayout.Space(5);
            GUILayout.EndVertical();

            if (parseStrings.Count == 0)
                return;

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);

            DisplaySheetItems();

            GUILayout.Space(5);
            GUILayout.EndVertical();

            #endregion
        }

        private void DisplaySheetOutput()
        {
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.black;

            isShowSheetOutput = GUILayout.Toggle(isShowSheetOutput, "Show Sheet Output");

            GUILayout.Space(5);

            if (!isShowSheetOutput)
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                return;
            }

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Space(5f);

            sheetOutputScrollPosition = EditorGUILayout.BeginScrollView(sheetOutputScrollPosition, GUILayout.Height(100));

            GUILayout.Label(callbackText, GetStyle(TextAnchor.MiddleLeft, FontStyle.Normal));

            EditorGUILayout.EndScrollView();

            GUILayout.Space(5f);
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        private void DisplaySheetItems()
        {
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.black;

            isShowSheetItems = GUILayout.Toggle(isShowSheetItems, "Show Sheet Parsing Items");

            if (!isShowSheetItems)
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                return;
            }

            GUILayout.Space(5);

            sheetItemsScrollPosition = EditorGUILayout.BeginScrollView(sheetItemsScrollPosition);

            for (int l = 0; l < parseStrings.Count; l++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                string[] rows = parseStrings.ElementAt(l).Value;
                GUILayout.Label($"Line: {l} | Columns: {rows.Length}", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));
                GUILayout.Space(2.5f);

                for (int r = 0; r < rows.Length; r++)
                    GUILayout.Label($"Column: {r} | {rows[r]}");

                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        #region Propperties Metods

        private bool GetFieldsParsingPropperties(object targetClass, out List<FieldsStruct> markersStorages, bool isSort = true, bool isShowProcess = false)
        {
            markersStorages = new();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] fields = targetClass.GetType().GetFields(flags);

            ConverterLog.Log($"Fields: {fields.Length}", isShowProcess);

            if (!ConverterSupports.TryGetCustomAttributes(fields, out markersStorages))
                return false;

            ConverterLog.OutputMarkersStruct(markersStorages, "Do: Field", "Attribute", isShowProcess);

            if (isSort)
                markersStorages.Sort((item1, item2) => item1.MarkerAttribute.Column.CompareTo(item2.MarkerAttribute.Column));

            ConverterLog.OutputMarkersStruct(markersStorages, "To -> Field", "Attribute", isShowProcess);

            return markersStorages.Count > 0;
        }

        private bool GetConstructors(object targetClass, out List<ConstructorsStruct> markersStorages, bool isShowProcess = false)
        {
            markersStorages = new();
            ConstructorInfo[] ctors = targetClass.GetType().GetConstructors();

            bool result = ConverterSupports.TryGetCustomAttributes(ctors, out markersStorages);
            ConverterLog.OutputConstructorsStruct(markersStorages, isShowProcess);

            return result;
        }

        private bool GetTargetConstructor(List<ConstructorsStruct> markersStorages, out ConstructorsStruct targetConstructor)
        {
            targetConstructor = new();
            if (markersStorages.Count == 0)
            {
                Debug.LogError($"Constructors List Is Null");
                return false;
            }

            ConverterLog.OutputConstructorsStruct(markersStorages, true);

            markersStorages.Sort((item1, item2) => item2.MarkerAttribute.Columns.Length.CompareTo(item1.MarkerAttribute.Columns.Length));

            ConverterLog.OutputConstructorsStruct(markersStorages, true);

            targetConstructor = markersStorages[0];
            return targetConstructor.MarkerAttribute != null;
        }

        private bool GetConstructorParsingPropperties(ConstructorsStruct targetConstructor, out Dictionary<int, ConstructorsStruct> constructorParsePropperties)
        {
            constructorParsePropperties = new();

            for (int c = 0; c < targetConstructor.MarkerAttribute.Columns.Length; c++)
            {
                if (constructorParsePropperties.ContainsKey(targetConstructor.MarkerAttribute.Columns[c]))
                {
                    Debug.LogError($"Marker Has Already Column {targetConstructor.MarkerAttribute.Columns[c]} In Constructor: {targetConstructor.MemberInfo} | Object: {targetConstructor.MarkerAttribute}");
                    return false;
                }

                constructorParsePropperties.Add(targetConstructor.MarkerAttribute.Columns[c], targetConstructor);
            }

            return constructorParsePropperties.Count > 0;
        }

        private void GetParsingPropperties(object targetClass,
            out Dictionary<int, ConstructorsStruct> constructorParsePropperties,
            out Dictionary<int, FieldsStruct> fieldsParsePropperties)
        {
            constructorParsePropperties = new();
            fieldsParsePropperties = new();

            bool hasConstructors = GetConstructors(targetClass, out List<ConstructorsStruct> constructorsStorages, true);
            bool hasFields = GetFieldsParsingPropperties(targetClass, out List<FieldsStruct> fieldsStorages, true);

            if (hasConstructors)
            {
                hasConstructors = GetTargetConstructor(constructorsStorages, out ConstructorsStruct targetConstructor);

                if (hasConstructors)
                    hasConstructors = GetConstructorParsingPropperties(targetConstructor, out constructorParsePropperties);
            }

            if (!hasConstructors)
                Debug.Log($"Not has Constructors Propperties");
            else
                ConverterLog.OutputDictionary(constructorParsePropperties, "Int", "Struct", true);

            if (!hasFields)
                Debug.Log($"Not Has Fields Propperties");
            else
            {
                for (int m = 0; m < fieldsStorages.Count; m++)
                {
                    fieldsParsePropperties.Add(fieldsStorages[m].MarkerAttribute.Column, fieldsStorages[m]);
                }
            }

        }

        #endregion

        #region Parse Metods

        private void ParseText()
        {
            //GetParsingPropperties(targetClass, out constructorsParsePropperties, out fieldsParsePropperties);
            ParseString();
        }

        private void ParseString()
        {
            parseStrings.Clear();

            if (callbackText == null || callbackText == default)
                return;

            List<string> lines = new(callbackText.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));

            for (int l = 0; l < lines.Count; l++)
            {
                lines[l].CheckLineForComplexString(out List<string> columns);
                parseStrings.Add(lines[l], columns.ToArray());
            }
        }

        #endregion
    }
}
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
        private static Vector2 windowSizeMin = new Vector2(450f, 600f);
        private static Vector2 windowSizeMax = new Vector2(600f, 900f);

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
        private Vector2 outputProppertiesScrollPosition = Vector2.zero;
        private Vector2 sheetItemsScrollPosition = Vector2.zero;

        private bool isShowSheetOutput = false;
        private bool isParsingProppetiesOutput = false;
        private bool isShowSheetItems = false;

        private Object tempObject;
        private Object targetObject;

        ConstructorsStruct constructorsParsePropperties = new();
        Dictionary<int, FieldsStruct> fieldsParsePropperties = new();

        public SimpleGoogleSheetsParserWindow()
        {
            titleText = windowTitle;
            mainScrollPosition = Vector2.zero;
        }

        [MenuItem("My Tools/Simple Resources Item Info Converter Window")]
        public static void ShowWindow()
        {
            OpenWindow(windowTitle, out SimpleGoogleSheetsParserWindow window);
            window.maxSize = windowSizeMax;
            window.minSize = windowSizeMin; 
        }

        protected override void Reconnect()
        {
            base.Reconnect(); 

            sheetOutputScrollPosition = Vector2.zero;
            isShowSheetOutput = false;
            
            ResetParseItems();
        }

        #region Draw Header

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

        private void DisplayReconnect()
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Space(5);

            if (GUILayout.Button("Reconect To Loader"))
                Reconnect();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        #endregion

        protected override void DisplayGUI()
        {
            DrawLogo();

            GUILayout.Space(85);

            DisplayReconnect();

            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            ContentDisplay();

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        #region Main Display

        private void ContentDisplay()
        {
            bool hasCallbackText = callbackText != null && callbackText != "";

            DisplayOutput(hasCallbackText);

            GUILayout.Space(10);

            ProppertiesDisplay();

            GUILayout.Space(10);

            OutputParsingDisplay(hasCallbackText);
        }

        #endregion

        #region Main Display/Block 1

        private void DisplayOutput(bool hasCallbackText)
        {
            if (hasCallbackText)
            {
                GUILayout.Space(10);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5f);

                DisplaySheetOutput();

                GUILayout.Space(5f);
                GUILayout.EndVertical();
            }
        }

        //  1.1
        private void DisplaySheetOutput()
        {
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.black;

            isShowSheetOutput = GUILayout.Toggle(isShowSheetOutput, "Show Sheet Output");

            if (!isShowSheetOutput)
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                return;
            }

            GUILayout.Space(5);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            sheetOutputScrollPosition = EditorGUILayout.BeginScrollView(sheetOutputScrollPosition, GUILayout.Height(100));

            GUILayout.Label(callbackText);

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        #endregion

        #region Main Display/Block 2

        private void ProppertiesDisplay()
        {
            //GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Parsing Propperties") && targetObject != null)
            {
                GetParsingPropperties(targetObject, out constructorsParsePropperties, out fieldsParsePropperties);
                tempObject = targetObject;
            }

            targetObject = EditorGUILayout.ObjectField(targetObject, typeof(ScriptableObject));

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Clear Parsing Propperties"))
            {
                constructorsParsePropperties.MarkerAttribute = null;
                constructorsParsePropperties.MemberInfo = null;

                isParsingProppetiesOutput = false;
                outputProppertiesScrollPosition = Vector2.zero;

                fieldsParsePropperties.Clear();
            }

            OutputParsingPropperties();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        //  2.1
        private void GetParsingPropperties(object targetClass,
            out ConstructorsStruct constructorParsePropperties,
            out Dictionary<int, FieldsStruct> fieldsParsePropperties)
        {
            constructorParsePropperties = new();
            fieldsParsePropperties = new();

            bool hasConstructors = GetConstructors(targetClass, out List<ConstructorsStruct> constructorsStorages, true);
            bool hasFields = GetFields(targetClass, out List<FieldsStruct> fieldsStorages, true);

            if (hasConstructors)
                hasConstructors = GetTargetConstructor(constructorsStorages, out constructorParsePropperties);

            if (!hasConstructors)
                Debug.Log($"Not has Constructors Propperties");
            else
                ConverterLog.OutputConstructorStruct(constructorParsePropperties, true);

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

        //  2.1.1
        private bool GetConstructors(object targetClass, out List<ConstructorsStruct> markersStorages, bool isShowProcess = false)
        {
            ConstructorInfo[] ctors = targetClass.GetType().GetConstructors();
            bool result = ConverterSupports.TryGetCustomAttributes(ctors, out markersStorages);

            ConverterLog.OutputConstructorsStruct(markersStorages, isShowProcess);

            return result;
        }

        //  2.1.2
        private bool GetFields(object targetClass, out List<FieldsStruct> markersStorages, bool isSort = true, bool isShowProcess = false)
        {
            markersStorages = new();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] fields = targetClass.GetType().GetFields(flags);

            ConverterLog.Log($"Fields: {fields.Length}", isShowProcess);

            if (!ConverterSupports.TryGetCustomAttributes(fields, out markersStorages))
                return false;

            for (int m = 0; m < markersStorages.Count; m++)
                markersStorages[m].MarkerAttribute.Initialization(markersStorages[m].MemberInfo, markersStorages[m].MemberInfo.GetValue(targetClass).GetType());

            ConverterLog.OutputMarkersStruct(markersStorages, "Do: Field", "Attribute", isShowProcess);

            if (isSort)
                markersStorages.Sort((item1, item2) => item1.MarkerAttribute.Column.CompareTo(item2.MarkerAttribute.Column));

            ConverterLog.OutputMarkersStruct(markersStorages, "To -> Field", "Attribute", isShowProcess);

            return markersStorages.Count > 0;
        }

        //  2.1.3
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

            ParameterInfo[] parameters = targetConstructor.MemberInfo.GetParameters();
            targetConstructor.MarkerAttribute.Initialization(targetConstructor.MemberInfo, parameters);

            return targetConstructor.MarkerAttribute != null;
        }

        //  2.2
        private void OutputParsingPropperties()
        {
            if (constructorsParsePropperties.MarkerAttribute == null && constructorsParsePropperties.MemberInfo == null
                && fieldsParsePropperties.Count == 0)
                return;

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.black;

            GUILayout.Space(5);

            isParsingProppetiesOutput = GUILayout.Toggle(isParsingProppetiesOutput, "Show Parsing Propperties Output");

            if (!isParsingProppetiesOutput)
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                //GUILayout.Space(5);
                return;
            }

            GUILayout.Space(5);

            float min = 0f;
            float max = 0f;

            float headerHeight = 4f + 6f + 5f + 21f;
            //float headerHeight = 35f;
            float titleHeight = 20f;

            int elements = 1;

            float minConstructor = 0f;
            float maxConstructor = 0f;
            float offcet = 0f;

            if (constructorsParsePropperties.MarkerAttribute != null && constructorsParsePropperties.MemberInfo != null)
            {
                minConstructor = headerHeight + Mathf.Min(constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length, elements) * titleHeight;
                maxConstructor = headerHeight + constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length * titleHeight;
                offcet += 4f;
            }

            float minFields = 0f;
            float maxFields = 0f;

            if (fieldsParsePropperties.Count > 0)
            {
                minFields = headerHeight + Mathf.Min(fieldsParsePropperties.Count, elements) * titleHeight;
                maxFields = headerHeight + (fieldsParsePropperties.Count) * titleHeight;
                offcet += 4f;
            }

            min = minConstructor + minFields;
            max = maxConstructor + maxFields;

            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            outputProppertiesScrollPosition = EditorGUILayout.BeginScrollView(outputProppertiesScrollPosition, GUILayout.ExpandHeight(false), GUILayout.MinHeight(min), GUILayout.MaxHeight(max + offcet));

            //GUILayout.ExpandHeight(false), GUILayout.MinHeight(minHeight), GUILayout.MaxHeight(maxHeight));

            if (constructorsParsePropperties.MarkerAttribute != null && constructorsParsePropperties.MemberInfo != null)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(maxConstructor), GUILayout.ExpandHeight(false));
                //GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(maxConstructor - 8.5f));
                GUILayout.Space(2.5f);

                GUILayout.Label($"Constructor Propperties: {constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length}", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

                GUILayout.Space(2.5f);

                DisplayConstructorPropperties(constructorsParsePropperties);

                GUILayout.Space(6f);
                GUILayout.EndVertical();
            }

            if (fieldsParsePropperties.Count > 0)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(maxFields), GUILayout.ExpandHeight(false));
                //GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(maxConstructor));
                GUILayout.Space(2.5f);

                GUILayout.Label($"Fields Propperties: {fieldsParsePropperties.Count}", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

                GUILayout.Space(2.5f);

                DisplayFieldsPropperties(fieldsParsePropperties);

                GUILayout.Space(6f);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        //  2.2.1
        private void DisplayConstructorPropperties(ConstructorsStruct propperties)
        {
            LoadConstructorMarkerAttribute attribute = propperties.MarkerAttribute;

            for (int i = 0; i < attribute.ArgumentsInfos.Length; i++)
                GUILayout.Label($"Element {i} | Type: {attribute.ArgumentsInfos[i].ParameterType} / Column: {attribute.Columns[i]} / Name: {attribute.ArgumentsInfos[i].Name}");
        }

        //  2.2.2
        private void DisplayFieldsPropperties(Dictionary<int, FieldsStruct> propperties)
        {
            for (int i = 0; i < propperties.Count; i++)
            {
                LoadMarkerAttribute attribute = propperties.ElementAt(i).Value.MarkerAttribute;
                GUILayout.Label($"Element {i} | Type: {attribute.FieldArgument} / Column: {attribute.Column}");
            }
        }

        #endregion

        #region Main Display/Block 3

        private void OutputParsingDisplay(bool hasCallbackText)
        {
            if (hasCallbackText)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);

                if (GUILayout.Button("Parse Output To Items"))
                    ParseText();

                if (GUILayout.Button("Clear Parse Strings"))
                    ResetParseItems();

                if (parseStrings.Count != 0)
                    DisplaySheetItems();

                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
        }

        //  3.1
        private void ParseText()
        {
            ParseString();
        }

        //  3.1.1
        private void ParseString()
        {
            parseStrings.Clear();

            if (callbackText == null || callbackText == default)
                return;

            List<string> lines = new(callbackText.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));

            if (lines.Count < 2)
                return;

            for (int l = 1; l < lines.Count; l++)
            {
                lines[l].CheckLineForComplexString(out List<string> columns);

                if (parseStrings.ContainsKey(lines[l]))
                {
                    Debug.Log($"Error | Lines {lines[l]} Has Been Added");
                    continue;
                }

                parseStrings.Add(lines[l], columns.ToArray());
            }
        }

        //  3.2
        private void ResetParseItems()
        {
            isShowSheetItems = false;
            sheetItemsScrollPosition = Vector2.zero;

            parseStrings.Clear();
        }

        //  3.3
        private void DisplaySheetItems()
        {
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.black;

            GUILayout.Space(5);

            isShowSheetItems = GUILayout.Toggle(isShowSheetItems, "Show Sheet Parsing Items");

            if (!isShowSheetItems)
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
                return;
            }

            GUILayout.Space(5);

            int itemHeight = 135;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            sheetItemsScrollPosition = EditorGUILayout.BeginScrollView(sheetItemsScrollPosition, GUILayout.MinHeight(itemHeight * Mathf.Min(2, parseStrings.Count)), GUILayout.MaxHeight(itemHeight * Mathf.Min(parseStrings.Count)));

            for (int l = 0; l < parseStrings.Count; l++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                string[] rows = parseStrings.ElementAt(l).Value;
                GUILayout.Label($"Line: {l} | Columns: {rows.Length}", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));
                GUILayout.Space(2.5f);

                for (int r = 0; r < rows.Length; r++)
                    GUILayout.Label($"Column: {r} | {rows[r]}");

                GUILayout.Space(6f);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        #endregion
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public sealed class SimpleGoogleSheetsParserWindow : BaseParserWindow
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

        Dictionary<string, string[]> parseLines = new();

        private Vector2 mainScrollPosition = Vector2.zero;

        private Vector2 sheetOutputScrollPosition = Vector2.zero;
        private Vector2 outputProppertiesScrollPosition = Vector2.zero;
        private Vector2 sheetItemsScrollPosition = Vector2.zero;

        private bool isShowSheetOutput = false;
        private bool isParsingProppetiesOutput = false;
        private bool isShowSheetItems = false;

        private Object targetObject;

        ConstructorsStruct constructorsParsePropperties = new();
        Dictionary<int, FieldsStruct> fieldsParsePropperties = new();

        List<Object> objectsData = new();

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

            objectsData.Clear();
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

            DisplayPropperties();

            GUILayout.Space(10);

            DisplayOutputParsing(hasCallbackText);

            GUILayout.Space(10);

            DisplayParsingAndCreate();
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

        private void DisplayPropperties()
        {
            //GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Parsing Propperties") && targetObject != null)
                targetObject.GetParsingPropperties(out constructorsParsePropperties, out fieldsParsePropperties, true);

            //targetObject = EditorGUILayout.ObjectField(targetObject, typeof(ScriptableObject));

            Object tempObject = EditorGUILayout.ObjectField(targetObject, typeof(ScriptableObject));

            if (tempObject != targetObject)
            {
                ClearParsingPropperties();
                targetObject = tempObject;
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Clear Parsing Propperties"))
                ClearParsingPropperties();

            OutputParsingPropperties();

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }

        //  2.0
        private void ClearParsingPropperties()
        {
            constructorsParsePropperties.MarkerAttribute = null;
            constructorsParsePropperties.MemberInfo = null;

            isParsingProppetiesOutput = false;
            outputProppertiesScrollPosition = Vector2.zero;

            fieldsParsePropperties.Clear();
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

        private void DisplayOutputParsing(bool hasCallbackText)
        {
            if (hasCallbackText)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);

                if (GUILayout.Button("Parse Output To Line Items"))
                    ParseText();

                if (GUILayout.Button("Clear Parse Lines"))
                    ResetParseItems();

                if (parseLines.Count != 0)
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
            parseLines.Clear();

            if (callbackText == null || callbackText == default)
                return;

            List<string> lines = new(callbackText.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));

            if (lines.Count < 2)
                return;

            for (int l = 1; l < lines.Count; l++)
            {
                lines[l].ParseStringToColumns(out List<string> columns);

                if (!columns.CheckRowsForData())
                {
                    //Debug.Log($"Error | Line {lines[l]} Data Is Null");
                    continue;
                }


                if (parseLines.ContainsKey(columns[0]))
                {
                    Debug.Log($"Error | Lines {lines[l]} Has Been Added");
                    continue;
                }

                parseLines.Add(columns[0], columns.ToArray());
            }
        }

        //  3.2
        private void ResetParseItems()
        {
            isShowSheetItems = false;
            sheetItemsScrollPosition = Vector2.zero;

            parseLines.Clear();
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
            sheetItemsScrollPosition = EditorGUILayout.BeginScrollView(sheetItemsScrollPosition, GUILayout.MinHeight(itemHeight * Mathf.Min(2, parseLines.Count)), GUILayout.MaxHeight(itemHeight * Mathf.Min(parseLines.Count)));

            for (int l = 0; l < parseLines.Count; l++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                string[] rows = parseLines.ElementAt(l).Value;
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

        #region Main Display/Block 4

        private void DisplayParsingAndCreate()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5f);

            if (!CheckByAvailable(out List<string> outWarnings))
            {
                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.black;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5f);

                GUILayout.Label("Warnings", GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));
                DrawWarnings(outWarnings);

                GUILayout.Space(5f);
                GUILayout.EndVertical();

                GUILayout.Space(5f);

                GUI.backgroundColor = Color.white;
                GUI.contentColor = Color.white;
            }
            else
            {
                if (GUILayout.Button("Parse Lines To Data Items"))
                {
                    ParseLinesToDataItems();
                }
            }

            if (GUILayout.Button("Clear Items Data"))
                objectsData.Clear();

            GUILayout.Space(5f);
            GUILayout.EndVertical();

            for (int i = 0; i < objectsData.Count; i++)
            {
                //Debug.Log($"{i} | {objects[i]} | Type: {objects[i].GetType()}");
                EditorGUILayout.ObjectField(objectsData[i], typeof(Object));
            }

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        //  4.1
        private bool CheckByAvailable(out List<string> outWarnings)
        {
            outWarnings = new();

            if (string.IsNullOrWhiteSpace(callbackText))
                outWarnings.Add("Google Sheet Text Is Null");

            if (targetObject == null)
                outWarnings.Add("Target Object Is Null");

            if (parseLines.Count == 0)
                outWarnings.Add("Parse Strings Is Null");

            if ((constructorsParsePropperties.MarkerAttribute == null || constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length == 0) && fieldsParsePropperties.Count == 0)
                outWarnings.Add("Parameters By Parse is Null");

            return outWarnings.Count == 0;
        }

        //  4.2
        private void DrawWarnings(List<string> warnings)
        {
            for (int i = 0; i < warnings.Count; i++)
                GUILayout.Label(warnings[i]);
        }

        //  4.3
        private void ParseLinesToDataItems()
        {
            objectsData.Clear();
            System.Type type = targetObject.GetType();

            for (int i = 0; i < parseLines.Count; i++)
            {
                Object currentObj;
                if (parseLines.ElementAt(i).Value.TryParseLineToConstructorArguments(constructorsParsePropperties, out object[] constructorArguments))
                    currentObj = (Object)System.Activator.CreateInstance(type, constructorArguments);
                else
                    currentObj = (Object)System.Activator.CreateInstance(type);

                if (parseLines.ElementAt(i).Value.TryParseLineToFields(fieldsParsePropperties, out Dictionary<int, object> outFieldsValues))
                {
                    for (int m = 0; m < outFieldsValues.Count; m++)
                    {
                        int markerColumn = outFieldsValues.ElementAt(m).Key;

                        if (fieldsParsePropperties.ContainsKey(markerColumn))
                            fieldsParsePropperties[markerColumn].MemberInfo.SetValue(currentObj, outFieldsValues.ElementAt(m).Value);
                    }
                }

                objectsData.Add(currentObj);
            }
        }

        #endregion
    }
}
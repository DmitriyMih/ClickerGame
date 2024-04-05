using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GoogleSheetLoaderSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public sealed class SimpleGoogleSheetsParserWindow : EditorWindow
    {
        private static Vector2 windowSizeMin = new Vector2(450f, 600f);
        private static Vector2 windowSizeMax = new Vector2(600f, 900f);

        const string headerPath = "Assets/BaseGoogleSheetLoader/System/Sprites/ParserHeader.png";
        const string iconPath = "Assets/BaseGoogleSheetLoader/System/Sprites/LogoIcon.png";

        Texture2D IconTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        Texture2D LogoTexture => AssetDatabase.LoadAssetAtPath<Texture2D>(headerPath);

        private string callbackText;
        Dictionary<string, string[]> parseLines = new();

        private Vector2 mainScrollPosition = Vector2.zero;

        private Vector2 sheetOutputScrollPosition = Vector2.zero;
        private Vector2 outputProppertiesScrollPosition = Vector2.zero;
        private Vector2 sheetItemsScrollPosition = Vector2.zero;

        private bool isShowSettings = false;

        private bool isShowSheetOutput = false;
        private bool isParsingProppetiesOutput = false;
        private bool isShowSheetItems = false;
        private bool isShowObjectsData = false;

        private bool isShowLoadProcessLog = false;
        private bool isShowParsingProppertiesProcessLog = false;

        private Object targetObject;

        ConstructorsStruct constructorsParsePropperties = new();
        Dictionary<int, FieldsStruct> fieldsParsePropperties = new();

        List<Object> objectsData = new();

        public SimpleGoogleSheetsParserWindow() => Reconnect();
        private void OnDisable() => GoogleSheetLoaderWindow.LoadCallback -= LoadCallback;

        [MenuItem("My Tools/Simple Google Sheets Parser")]
        public static void ShowWindow()
        {
            SimpleGoogleSheetsParserWindow window = GetWindow<SimpleGoogleSheetsParserWindow>();
            window.titleContent = new GUIContent("Simple Google Sheets Parser");

            window.maxSize = windowSizeMax;
            window.minSize = windowSizeMin;
        }

        private void Reconnect()
        {
            callbackText = null;

            GoogleSheetLoaderWindow.LoadCallback -= LoadCallback;
            GoogleSheetLoaderWindow.LoadCallback += LoadCallback;

            sheetOutputScrollPosition = Vector2.zero;
            isShowSheetOutput = false;

            ResetParseItems();

            objectsData.Clear();
        }

        private void LoadCallback(string callback)
        {
            callbackText = callback;
            SupportLog.Log($"Load Callback {callback}", isShowLoadProcessLog);
        }

        private void OnGUI()
        {
            WindowSupports.DrawLogo(LogoTexture, IconTexture);

            DrawSettingsBlock();

            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            ContentDisplay();

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        #region Main Display/Settings Block


        private void DrawSettingsBlock()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5);

            if (GUILayout.Button("Reconect To Loader"))
                Reconnect();

            GUILayout.Space(5);

            WindowSupports.SetColorState(true);

            isShowSettings.DrawToggle("Show Settings");

            WindowSupports.SetColorState(false);

            if (!isShowSettings)
            {
                GUILayout.Space(5);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.Space(5);

            WindowSupports.SetColorState(true);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2.5f);


            GUILayout.Label($"Settings", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

            GUILayout.Space(2.5f);

            isShowLoadProcessLog.DrawToggle("Show Load Process Log");
            isShowParsingProppertiesProcessLog.DrawToggle("Show Parsing Propperties Process Log");

            //showLoadLog = GUILayout.Toggle(showLoadLog, "Show Load Log");
            //showLoadLog = GUILayout.Toggle(showLoadLog, "Show Load Log");
            //showLoadLog = GUILayout.Toggle(showLoadLog, "Show Load Log");

            GUILayout.Space(2.5f);
            GUILayout.EndVertical();

            WindowSupports.SetColorState(false);

            //  log в консоль output
            //  log в консоль parser
            //  log в консоль converter
            //  log в консоль

            //  кнопка фулл рестарт

            GUILayout.Space(5);
            GUILayout.EndVertical();
        }


        #endregion

        #region Main Display

        private void ContentDisplay()
        {
            bool hasCallbackText = !string.IsNullOrWhiteSpace(callbackText);

            DisplayCallbackText(hasCallbackText);

            GUILayout.Space(10);

            DisplayProppertiesBlock();

            GUILayout.Space(10);

            DisplayInititalizationParsingBlock(hasCallbackText);

            GUILayout.Space(10);

            DisplayParsingAndCreateBlock();
        }

        #endregion

        #region Main Display/Block 1

        private void DisplayCallbackText(bool hasCallbackText)
        {
            if (!hasCallbackText)
                return;

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2.5f);

            DisplaySheetOutput();

            GUILayout.Space(2.5f);
            GUILayout.EndVertical();
        }

        //  1.1
        private void DisplaySheetOutput()
        {
            WindowSupports.SetColorState(true);

            isShowSheetOutput.DrawToggle("Show Sheet Output");

            if (!isShowSheetOutput)
            {
                GUILayout.Space(2.5f);
                WindowSupports.SetColorState(false);
                return;
            }

            GUILayout.Space(5);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            sheetOutputScrollPosition = EditorGUILayout.BeginScrollView(sheetOutputScrollPosition, GUILayout.Height(100));

            GUILayout.Label(callbackText);

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            WindowSupports.SetColorState(false);
        }

        #endregion

        #region Main Display/Block 2

        private void DisplayProppertiesBlock()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false));
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Get Parsing Propperties") && targetObject != null)
                targetObject.GetParsingPropperties(out constructorsParsePropperties, out fieldsParsePropperties, isShowParsingProppertiesProcessLog);

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

        //  2.1
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

            WindowSupports.SetColorState(true);

            GUILayout.Space(5);

            isParsingProppetiesOutput.DrawToggle("Show Parsing Propperties Output");

            if (!isParsingProppetiesOutput)
            {
                WindowSupports.SetColorState(false);
                return;
            }

            GUILayout.Space(5);

            GetParseBlockHeight(out float min, out float max, out float maxConstructor, out float maxFields, out float offcet);

            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            outputProppertiesScrollPosition = EditorGUILayout.BeginScrollView(outputProppertiesScrollPosition, GUILayout.ExpandHeight(false), GUILayout.MinHeight(min), GUILayout.MaxHeight(max + offcet));

            if (constructorsParsePropperties.MarkerAttribute != null && constructorsParsePropperties.MemberInfo != null)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(maxConstructor), GUILayout.ExpandHeight(false));
                GUILayout.Space(2.5f);

                GUILayout.Label($"Constructor Propperties: {constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length}", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

                GUILayout.Space(2.5f);

                DisplayConstructorPropperties(constructorsParsePropperties);

                GUILayout.Space(2.5f);
                GUILayout.EndVertical();
            }

            if (fieldsParsePropperties.Count > 0)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(maxFields), GUILayout.ExpandHeight(false));
                GUILayout.Space(2.5f);

                GUILayout.Label($"Fields Propperties: {fieldsParsePropperties.Count}", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

                GUILayout.Space(2.5f);

                DisplayFieldsPropperties(fieldsParsePropperties);

                GUILayout.Space(2.5f);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            WindowSupports.SetColorState(false);
        }

        //  2.2.0
        private void GetParseBlockHeight(out float min, out float max, out float maxConstructor, out float maxFields, out float offcet)
        {
            offcet = 0f;

            float headerHeight = 4f + 2.5f + 5f + 21f;
            float titleHeight = 20f;

            int elements = 1;

            float minConstructor = 0f;
            maxConstructor = 0f;

            if (constructorsParsePropperties.MarkerAttribute != null && constructorsParsePropperties.MemberInfo != null)
            {
                minConstructor = headerHeight + Mathf.Min(constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length, elements) * titleHeight;
                maxConstructor = headerHeight + constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length * titleHeight;
                offcet += 4f;
            }

            float minFields = 0f;
            maxFields = 0f;

            if (fieldsParsePropperties.Count > 0)
            {
                minFields = headerHeight + Mathf.Min(fieldsParsePropperties.Count, elements) * titleHeight;
                maxFields = headerHeight + (fieldsParsePropperties.Count) * titleHeight;
                offcet += 4f;
            }

            min = minConstructor + minFields;
            max = maxConstructor + maxFields;
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

        private void DisplayInititalizationParsingBlock(bool hasCallbackText)
        {
            if (hasCallbackText)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(5);

                if (GUILayout.Button("Parse The Text On The Line"))
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
                    Debug.Log($"Error | Line {lines[l]} Data Is Null");
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
            WindowSupports.SetColorState(true);

            GUILayout.Space(5);

            isShowSheetItems.DrawToggle("Show Sheet Parsing Items");

            if (!isShowSheetItems)
            {
                WindowSupports.SetColorState(false);
                return;
            }

            GUILayout.Space(5);

            int itemHeight = 135;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            sheetItemsScrollPosition = EditorGUILayout.BeginScrollView(sheetItemsScrollPosition, GUILayout.MinHeight(itemHeight * Mathf.Min(2, parseLines.Count)), GUILayout.MaxHeight(itemHeight * Mathf.Min(parseLines.Count)));

            for (int l = 0; l < parseLines.Count; l++)
            {
                string[] rows = parseLines.ElementAt(l).Value;

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                GUILayout.Label($"Line: {l} | Columns: {rows.Length}", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));

                GUILayout.Space(2.5f);

                for (int r = 0; r < rows.Length; r++)
                    GUILayout.Label($"Column: {r} | {rows[r]}");

                GUILayout.Space(2.5f);
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            WindowSupports.SetColorState(false);
        }

        #endregion

        #region Main Display/Block 4

        private void DisplayParsingAndCreateBlock()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5f);

            bool hasAvailable = CheckByAvailable(out List<string> outWarnings);

            if (!hasAvailable)
            {
                WindowSupports.SetColorState(true);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                GUILayout.Label("Take The Steps:", WindowSupports.GetStyle(TextAnchor.MiddleCenter, FontStyle.Bold, 14));
                DrawWarnings(outWarnings);

                GUILayout.Space(2.5f);
                GUILayout.EndVertical();

                WindowSupports.SetColorState(false);
            }

            GUILayout.Space(5f);

            if (GUILayout.Button("Parse Lines To Data Items") && hasAvailable)
                ParseLinesToDataItems();

            if (GUILayout.Button("Clear Items Data"))
                objectsData.Clear();

            if (objectsData.Count == 0)
            {
                GUILayout.Space(5f);
                GUILayout.EndVertical();

                return;
            }

            GUILayout.Space(5f);

            WindowSupports.SetColorState(true);

            isShowObjectsData.DrawToggle("Show Data Items");

            WindowSupports.SetColorState(false);

            if (isShowObjectsData)
            {
                GUILayout.Space(5f);

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                for (int i = 0; i < objectsData.Count; i++)
                    EditorGUILayout.ObjectField(objectsData[i], typeof(Object));

                GUILayout.Space(2.5f);
                GUILayout.EndVertical();
            }

            GUILayout.Space(5f);
            GUILayout.EndVertical();
        }

        //  4.1
        private bool CheckByAvailable(out List<string> outWarnings)
        {
            outWarnings = new();

            if (string.IsNullOrWhiteSpace(callbackText))
            {
                outWarnings.Add("Sheet Callback Text Is Null");
                outWarnings.Add("Load Sheet In Sheet Loader Window");
            }

            if (parseLines.Count == 0)
            {
                outWarnings.Add("Parse Strings Is Null");

                if (string.IsNullOrWhiteSpace(callbackText))
                    outWarnings.Add("Load Sheet In Sheet Loader Window\nParse Callback Text In Parser Window");
                else
                    outWarnings.Add("Parse Callback Text In Parser WIndow");
            }

            if (targetObject == null)
            {
                outWarnings.Add("Target Object Is Null");
                outWarnings.Add("Select Target Object In Parser Window");
            }

            if ((constructorsParsePropperties.MarkerAttribute == null || constructorsParsePropperties.MarkerAttribute.ArgumentsInfos.Length == 0) && fieldsParsePropperties.Count == 0)
            {
                outWarnings.Add("Parameters By Parse is Null");

                if (targetObject == null)
                    outWarnings.Add("Select Object In Parser Window\nParse Target Object In Parser Window");
                else
                    outWarnings.Add("Parse Target Object in Parser Window");
            }

            return outWarnings.Count == 0;
        }

        //  4.2
        private void DrawWarnings(List<string> warnings)
        {
            for (int i = 0; i < warnings.Count - 1; i += 2)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2.5f);

                GUILayout.Label(warnings[i], WindowSupports.GetStyle(fontStyle: FontStyle.Normal));
                GUILayout.Label(warnings[i + 1], WindowSupports.GetStyle(TextAnchor.MiddleRight, FontStyle.BoldAndItalic));

                GUILayout.Space(2.5f);
                GUILayout.EndVertical();

                GUILayout.Space(5);
            }
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
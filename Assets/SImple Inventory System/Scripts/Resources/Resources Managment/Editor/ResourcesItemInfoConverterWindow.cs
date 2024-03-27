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

    public class ResourcesItemInfoConverterWindow : BaseResourcesConverterWindow
    {
        Dictionary<string, string[]> parseStrings = new();
        public Vector2 scrollPosition = Vector2.zero;

        [MenuItem("My Tools/Simple Resources Item Info Converter Window")]
        public static void ShowWindow()
        {
            OpenWindow<ResourcesItemInfoConverterWindow>("Simple Resources Converter");
        }

        protected override void DisplayGUI()
        {
            base.DisplayGUI();

            if (GUILayout.Button("Parse Text"))
                ParseText();

            Output();
        }

        private void Output()
        {
            if (callbackText != null && callbackText != "")
                GUILayout.Label(callbackText, GetStyle(TextAnchor.MiddleLeft, FontStyle.Bold));

            if (parseStrings.Count == 0)
                return;

            GUILayout.Space(10);

            if(GUILayout.Button("Clear Parse Strings"))
                parseStrings.Clear();

            GUILayout.Space(10);

            GUILayout.BeginVertical("HelpBox");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int l = 0; l < parseStrings.Count; l++)
            {
                GUILayout.BeginVertical("HelpBox");
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
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
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
            Object targetClass = new SimpleResourcesItemInfo();

            Dictionary<int, ConstructorsStruct> constructorsParsePropperties;
            Dictionary<int, FieldsStruct> fieldsParsePropperties;

            GetParsingPropperties(targetClass, out constructorsParsePropperties, out fieldsParsePropperties);
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
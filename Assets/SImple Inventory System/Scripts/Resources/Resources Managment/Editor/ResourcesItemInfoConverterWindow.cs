using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SimpleResourcesSystem.SimpleItemSystem;
using System.Reflection;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public class ResourcesItemInfoConverterWindow : BaseResourcesConverterWindow
    {
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
            //GetParsingPropperties(new SimpleResourcesItemInfo());
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

            return true;
        }

        private void GetConstructorParsingPropperties(ConstructorsStruct targetConstructor, out Dictionary<int, ConstructorsStruct> constructorParsePropperties)
        {
            constructorParsePropperties = new();
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
                if (GetTargetConstructor(constructorsStorages, out ConstructorsStruct targetConstructor))
                    hasConstructors = true;
                else
                    hasConstructors = false;
            }

            if (!hasConstructors)
                Debug.Log($"Not has Constructors Propperties");

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

            List<string> strings = new();

            for (int s = 0; s < strings.Count; s++)
                ParseString(strings[s]);
        }

        private void ParseString(string str)
        {

        }

        #endregion
    }
}
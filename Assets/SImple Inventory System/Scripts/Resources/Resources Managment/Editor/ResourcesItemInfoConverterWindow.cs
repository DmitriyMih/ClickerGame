using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using SimpleItemSystem;
    using System;
    using System.Reflection;

    public class ResourcesItemInfoConverterWindow : BaseResourcesConverterWindow
    {
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        [MenuItem("My Tools/Simple Resources Item Info Converter Window")]
        public static void ShowWindow()
        {
            OpenWindow<ResourcesItemInfoConverterWindow>("Simple Resources Converter");
        }

        protected override void DisplayGUI()
        {
            base.DisplayGUI();

            if (GUILayout.Button("Draw"))
                DisplayItem(new[] { new SimpleResourcesItemInfo() });

        }

        private bool GetFieldsParsingPropperties(object targetClass, out List<MarkersStorage<LoadMarkerAttribute, FieldInfo>> markersStorages)
        {
            markersStorages = new();

            Type type = targetClass.GetType();
            FieldInfo[] fields = type.GetFields(flags);

            Debug.Log("Fields:");
            if (!ConverterSupports.TryGetCustomAttributes(fields, out markersStorages))
                return false;

            ConverterSupports.OutputMarkersStruct(markersStorages, "Do: Field", "Attribute");
            markersStorages.Sort((item1, item2) => item1.MarkerAttribute.Column.CompareTo(item2.MarkerAttribute.Column));
            ConverterSupports.OutputMarkersStruct(markersStorages, "To -> Field", "Attribute");

            return markersStorages.Count > 0;
        }

        private void DisplayItem(object targetClass)
        {
            if (GetFieldsParsingPropperties(targetClass, out List<MarkersStorage<LoadMarkerAttribute, FieldInfo>> markersStorages))
            {
                Debug.LogError($"Not Get Propperties");
                return;
            }

            //for (int x = 0; x < fieldsDictionary.Count; x++)
            //{
            //    FieldInfo field = fieldsDictionary.ElementAt(x).Key;
            //    Debug.Log($"Field Value: {field.GetValue(classes[c])}");  //    value
            //    Debug.Log($"Field Type: {field.GetValue(classes[c]).GetType()}");  //    type
            //}

            //for (int x = 0; x < constructorsDictionary.Count; x++)
            //{
            //    ConstructorInfo constructor = constructorsDictionary.ElementAt(x).Key;
            //    //Debug.Log($"Field Value: {constructor.GetValue(classes[c])}");
            //}
        }
    }
}
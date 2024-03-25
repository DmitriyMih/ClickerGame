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
        [MenuItem("My Tools/Simple Resources Item Info Converter Window")]
        public static void ShowWindow()
        {
            OpenWindow<ResourcesItemInfoConverterWindow>("Simple Resources Converter");
        }

        protected override void DisplayGUI()
        {
            base.DisplayGUI();

            if (GUILayout.Button("Draw"))
                DisplayItem(new[] { typeof(SimpleResourcesItemInfo) });
            //DisplayItem(new[] { typeof(SimpleResourcesItemInfo), typeof(BaseResourceInfo) });
            //DisplayItem(new[] { typeof(BaseResourceInfo) });
        }

        private void DisplayItem<TClass>(TClass[] classes) where TClass : Type
        {
            Debug.Log(classes.Length);

            for (int i = 0; i < classes.Length; i++)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                //FieldInfo[] fields = classes[c].GetFields(flags);

                FieldInfo[] fields = classes[i].GetFields(flags);
                ConstructorInfo[] constructors = classes[i].GetConstructors();

                Dictionary<FieldInfo, LoadMarkerAttribute> fieldsDictionary = new();
                Dictionary<ConstructorInfo, LoadCunstructorMarkerAttribute> constructorsDictionary = new();

                Debug.Log("");
                Debug.Log($"Class: {classes[i]}");
                Debug.Log("-->");

                Debug.Log("Fields:");
                if (classes[i].TryGetCustomAttributes(fields, out fieldsDictionary))
                    classes[i].OutputDictionary(fieldsDictionary, "Field", "Attribute");

                Debug.Log("Constructors:");
                if (classes[i].TryGetCustomAttributes(constructors, out constructorsDictionary))
                    classes[i].OutputDictionary(constructorsDictionary, "Constructor Info", "Attribute");

                List<BaseMarkerAttribute> markers = new();
                markers.AddRange(fieldsDictionary.Values);
                markers.AddRange(constructorsDictionary.Values);

                for (int x = 0; x < markers.Count; x++)
                    Debug.Log($"{x} : {markers[x].Column}");

                Debug.Log("");
                markers.Sort((item1, item2) => item1.Column.CompareTo(item2.Column));

                for (int x = 0; x < markers.Count; x++)
                {
                    Debug.Log(markers[x].GetType().GetProperty("Column"));
                    Debug.Log($"{x} : {markers[x].Column}");
                }
            }
        }
    }
}
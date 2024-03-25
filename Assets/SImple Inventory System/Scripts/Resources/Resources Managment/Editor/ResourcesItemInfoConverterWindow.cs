using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

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
                if(classes[i].TryGetCustomAttributes(constructors, out constructorsDictionary))
                    classes[i].OutputDictionary(constructorsDictionary, "Constructor Info", "Attribute");

                //{
                //    Debug.Log($"Field {f} | {fields[f]}");
                //    //if (fields[f].TryGetCustomAttribute(fields[f], out LoadMarkerAttribute loadMarkerAttribute))
                //    //{
                //    //    Debug.Log($"Get By Field {fields[f]} | Attribute {loadMarkerAttribute}");

                //        //    if (attributesDictionary.ContainsKey(fields[f]))
                //        //    {
                //        //        Debug.LogError($"Field {fields[f]} Not Has Been Added To Dictionary");
                //        //        continue;
                //        //    }
                //        //    else
                //        //        attributesDictionary.Add(fields[f], loadMarkerAttribute);
                //        //}
                //        //else
                //        //    Debug.Log($"Not Get By Field {fields[f]}");
                //}
            }
        }
    }
}
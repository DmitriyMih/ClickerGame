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
        List<SimpleResourcesItemInfo> tempItemsInfo = new();

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
            //DisplayItem(new[] { typeof(BaseResourceInfo) });
            //DisplayItem(new[] { typeof(SimpleResourcesItemInfo), typeof(BaseResourceInfo) });
        }

        private void DisplayItem<TClass>(TClass[] classes) where TClass : Type
        {
            Debug.Log(classes.Length);

            for (int c = 0; c < classes.Length; c++)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                FieldInfo[] fields = classes[c].GetFields(flags);

                Dictionary<FieldInfo, LoadMarkerAttribute> attributesDictionary = new();

                Debug.Log("");
                Debug.Log($"Class: {classes[c]}");

                for (int f = 0; f < fields.Length; f++)
                {
                    if (fields[f].TryGetCustomAttribute(fields[f], out LoadMarkerAttribute loadMarkerAttribute))
                    {
                        Debug.Log($"Get By Field {fields[f]} | Attribute {loadMarkerAttribute}");

                        if (attributesDictionary.ContainsKey(fields[f]))
                        {
                            Debug.LogError($"Field {fields[f]} Not Has Been Added To Dictionary");
                            continue;
                        }
                        else
                            attributesDictionary.Add(fields[f], loadMarkerAttribute);
                    }
                    else
                        Debug.Log($"Not Get By Field {fields[f]}");

                    //object[] os = fields[f].GetCustomAttributes(typeof(LoadMarkerAttribute), false);
                    //Debug.Log($"Field - {fields[f]} / Type: {fields[f].FieldType} | {fields[f].GetCustomAttributes(typeof(LoadMarkerAttribute), false).Length}");
                    //Debug.Log($"Field I Count - {os.Length}");

                    //for (int i = 0; i < os.Length; i++)
                    //Debug.Log($"{f}.{i} | {os[i]}");
                }
            }
        }
    }
}
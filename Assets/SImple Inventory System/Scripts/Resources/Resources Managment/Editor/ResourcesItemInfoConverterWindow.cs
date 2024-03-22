using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using SimpleItemSystem;
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
                DisplayItem(new[] { typeof(TestParent)});
                //DisplayItem(new[] { typeof(SimpleResourcesItemInfo), typeof(BaseResourceInfo) });
        }
        
        private void DisplayItem<TClass>(TClass[] classes) where TClass : System.Type
        {
            Debug.Log(classes);
            List<FieldInfo> mi = new();

            for (int i = 0; i < classes.Length; i++)
                mi.AddRange(classes[i].GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));

            Debug.Log($"Start Draw {mi.Count}");

            for (int i = 0; i < mi.Count; i++)
                Debug.Log($"{i} | {mi[i]}");

            //Debug.Log(typeof(TClass));

            //for (int i = 0; i < classes.Count; i++)
            //Debug.Log(classes[i]);

            //List<SerializedProperty> sp = new List<SerializedProperty>();

            //FieldInfo[] mi = typeof(SimpleResourcesItemInfo).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            //Debug.Log($"Start Draw {mi.Length}");

            //for (int i = 0; i < mi.Length; i++)
            //{
            //    Debug.Log($"{i} | {mi[i]}");
            //    //if (mi[i].IsDefined(typeof(HideInInspector)) == false)
            //    //{

            //    //sp.Add(serializedObject.FindProperty(mi[i].Name));
            //    //}
            //}
        }
    }
}
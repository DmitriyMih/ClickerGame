using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;

    public struct MarkersStorage<TAttribute, TMember>
        where TAttribute : System.Attribute
        where TMember : MemberInfo
    {
        public TAttribute MarkerAttribute;
        public TMember MemberInfo;

        public MarkersStorage(TAttribute markerAttribute, TMember memberInfo)
        {
            MarkerAttribute = markerAttribute;
            MemberInfo = memberInfo;
        }
    }

    public static class ConverterSupports
    {
        public static void ParseString(this string str,
            Dictionary<int, ConstructorsStruct> constructorsPropperties,
            Dictionary<int, FieldsStruct> fieldsPropperties)
        {

        }

        #region Reflection Metods

        private static bool TryGetCustomAttribute<TAttribute>(this MemberInfo field, out TAttribute attribute) where TAttribute : System.Attribute
        {
            attribute = field.GetCustomAttribute<TAttribute>();
            return attribute != null;
        }

        public static bool TryGetCustomAttributes<TMember, TAttribute>(TMember[] memberInfos, out List<MarkersStorage<TAttribute, TMember>> markersStorage)
            where TAttribute : System.Attribute
            where TMember : MemberInfo
        {
            markersStorage = new();

            for (int m = 0; m < memberInfos.Length; m++)
                if (memberInfos[m].TryGetCustomAttribute(out TAttribute marker))
                    markersStorage.Add(new(marker, memberInfos[m]));

            //Debug.Log(markersStorage.Count);
            return markersStorage.Count > 0;
        }

        #endregion
    }

    public static class ConverterLog
    {
        #region Output Metods

        public static void Log(string message, bool isShow = true)
        {
            if (isShow)
                Debug.Log(message);
        }

        public static void OutputDictionary<TAttribute, TMember>(Dictionary<TMember, TAttribute> pairs, string keyTitle, string valueTitle, bool isShow = true)
                where TAttribute : System.Attribute
                where TMember : MemberInfo
        {
            for (int i = 0; i < pairs.Count; i++)
                Debug.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}");
        }

        public static void OutputMarkersStruct<TAttribute, TMember>(List<MarkersStorage<TAttribute, TMember>> markers, string memberTitle, string ttributeTitle, bool isShow = true)
            where TAttribute : BaseMarkerAttribute
            where TMember : MemberInfo
        {
            for (int i = 0; i < markers.Count; i++)
                ConverterLog.Log($"{markers[i].MarkerAttribute.Column} | {memberTitle}: {markers[i].MemberInfo} / {ttributeTitle}: {markers[i].MarkerAttribute}");
        }

        public static void OutputConstructorsStruct(List<ConstructorsStruct> markers, bool isShow = true)
        {
            for (int m = 0; m < markers.Count; m++)
            {
                ConverterLog.Log($"Constructor: {markers[m].MemberInfo} / {markers[m].MarkerAttribute.Columns}");
                ParameterInfo[] parameters = markers[m].MemberInfo.GetParameters();

                for (int p = 0; p < parameters.Length; p++)
                    ConverterLog.Log($"Param {parameters[p].Position} is named {parameters[p].Name} and is of type {parameters[p].ParameterType}");
            }
        }

        #endregion
    }
}
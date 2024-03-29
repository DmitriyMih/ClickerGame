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
        private static bool CheckStartComplexString(this string checkString) => checkString.Length > 0 && checkString[0] == '"';
        private static bool CheckEndComplexString(this string checkString) => checkString.Length > 0 && checkString[checkString.Length - 1] == '"';
       
        public static void CheckLineForComplexString(this string line, out List<string> outColumns)
        {
            List<string> columns = line.Split(",", System.StringSplitOptions.None).ToList();

            bool findComplexString = false;
            int complexStringId = -1;

            List<int> removeIndexes = new();

            for (int i = 0; i < columns.Count; i++)
            {
                if (!findComplexString)
                {
                    if (columns[i].CheckStartComplexString())
                    {
                        findComplexString = true;
                        complexStringId = i;

                        if (columns[i].CheckEndComplexString())
                        {
                            columns[complexStringId] = ClearingGoogleSheetMarkup(columns[complexStringId]);
                            findComplexString = false;
                            complexStringId = -1;
                        }
                    }
                }
                else
                {
                    if (columns[i].CheckEndComplexString())
                    {
                        findComplexString = false;

                        columns[complexStringId] += "," + columns[i];
                        removeIndexes.Add(i);

                        columns[complexStringId] = ClearingGoogleSheetMarkup(columns[complexStringId]);
                        complexStringId = -1;
                    }
                    else
                    {
                        columns[complexStringId] += "," + columns[i];
                        removeIndexes.Add(i);
                    }
                }
            }

            if (complexStringId > 0)
                columns[complexStringId] = ClearingGoogleSheetMarkup(columns[complexStringId]);

            for (int i = removeIndexes.Count - 1; i >= 0; i--)
                columns.RemoveAt(removeIndexes[i]);

            outColumns = new(columns);
        }

        public static string ClearingGoogleSheetMarkup(string row)
        {
            List<string> tempSplit = row.Trim().Split('"', System.StringSplitOptions.None).ToList();
            string outString = "";

            if (tempSplit[0].Length == 0)
                tempSplit.RemoveAt(0);

            if (tempSplit[tempSplit.Count - 1].Length == 0)
                tempSplit.RemoveAt(tempSplit.Count - 1);

            for (int i = 0; i < tempSplit.Count; i++)
            {
                if (tempSplit[i].Length == 0)
                {
                    if (outString.Length > 0 && outString[outString.Length - 1] == '"')
                        tempSplit.RemoveAt(i);
                    else
                        outString += '"';
                }
                else
                    outString += tempSplit[i];
            }

            return outString;
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

        public static void OutputMemberDictionary<TAttribute, TMember>(Dictionary<TMember, TAttribute> pairs, string keyTitle, string valueTitle, bool isShow = true)
                where TAttribute : System.Attribute
                where TMember : MemberInfo
        {
            for (int i = 0; i < pairs.Count; i++)
                ConverterLog.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}", isShow);
        }

        public static void OutputDictionary<TKey, TValue>(Dictionary<TValue, TKey> pairs, string keyTitle, string valueTitle, bool isShow = true)
        {
            for (int i = 0; i < pairs.Count; i++)
                ConverterLog.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}", isShow);
        }

        public static void OutputMarkersStruct<TAttribute, TMember>(List<MarkersStorage<TAttribute, TMember>> markers, string memberTitle, string ttributeTitle, bool isShow = true)
            where TAttribute : LoadMarkerAttribute
            where TMember : MemberInfo
        {
            for (int i = 0; i < markers.Count; i++)
                ConverterLog.Log($"{markers[i].MarkerAttribute.Column} | {memberTitle}: {markers[i].MemberInfo} / {ttributeTitle}: {markers[i].MarkerAttribute}");
        }

        public static void OutputConstructorStruct(ConstructorsStruct marker, bool isShow = true)
        {
                ConverterLog.Log($"Constructor: {marker.MemberInfo} / {marker.MarkerAttribute.Columns}");
                ParameterInfo[] parameters = marker.MemberInfo.GetParameters();

                for (int p = 0; p < parameters.Length; p++)
                    ConverterLog.Log($"Param {parameters[p].Position} is named {parameters[p].Name} and is of type {parameters[p].ParameterType}");
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
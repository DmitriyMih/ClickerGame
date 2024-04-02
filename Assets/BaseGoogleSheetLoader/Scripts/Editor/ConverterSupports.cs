using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

namespace SimpleResourcesSystem.ResourceManagementSystem
{
    using FieldsStruct = MarkersStorage<LoadMarkerAttribute, FieldInfo>;
    using ConstructorsStruct = MarkersStorage<LoadConstructorMarkerAttribute, ConstructorInfo>;
    using Object = UnityEngine.Object;

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

    public struct DataStruct
    {
        public List<object> DataObjects;
        public Type ObjectsType;

        public DataStruct(List<object> dataObjects, Type objectsType)
        {
            DataObjects = new(dataObjects);
            ObjectsType = objectsType;
        }
    }

    public static class ConverterSupports
    {
        public static bool TryParseByType<TType>(this string str, TType type, out object outObj) where TType : Type
        {
            outObj = null;

            if (string.IsNullOrWhiteSpace(str))
                return false;

            if (type.GetType().IsArray)
            {
                List<string> elements = str.Split(",", System.StringSplitOptions.None).ToList();
                List<object> outArray = new();

                Type elementType = type.GetElementType();

                for (int i = 0; i < elements.Count; i++)
                {
                    var element = Convert.ChangeType(elements[i], elementType);
                    outArray.Add(element);
                }

                outObj = outArray;
            }
            else
                outObj = Convert.ChangeType(str, type);

            Debug.Log($"Parse Out: {outObj} | Type: {outObj.GetType()}");
            return outObj != null;
        }

        //  2.1
        public static (bool, bool) GetParsingPropperties(this Object targetClass,
            out ConstructorsStruct constructorParsePropperties,
            out Dictionary<int, FieldsStruct> fieldsParsePropperties, bool showOutput)
        {
            constructorParsePropperties = new();
            fieldsParsePropperties = new();

            bool hasConstructors = targetClass.TryGetConstructors(out List<ConstructorsStruct> constructorsStorages, showOutput);
            bool hasFields = targetClass.TryGetFields(out List<FieldsStruct> fieldsStorages, showOutput);

            if (hasConstructors)
                hasConstructors = TryGetTargetConstructor(constructorsStorages, out constructorParsePropperties);

            if (!hasConstructors)
                Debug.Log($"Not has Constructors Propperties");
            else
                ConverterLog.OutputConstructorStruct(constructorParsePropperties, showOutput);

            if (!hasFields)
                Debug.Log($"Not Has Fields Propperties");
            else
            {
                for (int m = 0; m < fieldsStorages.Count; m++)
                    fieldsParsePropperties.Add(fieldsStorages[m].MarkerAttribute.Column, fieldsStorages[m]);
            }

            return (hasConstructors, hasFields);
        }

        //  2.1.1
        private static bool TryGetConstructors(this Object targetClass, out List<ConstructorsStruct> markersStorages, bool isShowProcess = false)
        {
            ConstructorInfo[] ctors = targetClass.GetType().GetConstructors();
            bool result = ConverterSupports.TryGetCustomAttributes(ctors, out markersStorages);

            ConverterLog.OutputConstructorsStruct(markersStorages, isShowProcess);

            return result;
        }

        private static bool TryGetFields(this Object targetClass, out List<FieldsStruct> markersStorages, bool isSort = true, bool isShowProcess = false)
        {
            markersStorages = new();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            FieldInfo[] fields = targetClass.GetType().GetFields(flags);

            ConverterLog.Log($"Fields: {fields.Length}", isShowProcess);

            if (!ConverterSupports.TryGetCustomAttributes(fields, out markersStorages))
                return false;

            for (int m = 0; m < markersStorages.Count; m++)
                markersStorages[m].MarkerAttribute.Initialization(markersStorages[m].MemberInfo, markersStorages[m].MemberInfo.GetValue(targetClass).GetType());

            ConverterLog.OutputMarkersStruct(markersStorages, "Do: Field", "Attribute", isShowProcess);

            if (isSort)
                markersStorages.Sort((item1, item2) => item1.MarkerAttribute.Column.CompareTo(item2.MarkerAttribute.Column));

            ConverterLog.OutputMarkersStruct(markersStorages, "To -> Field", "Attribute", isShowProcess);

            return markersStorages.Count > 0;
        }

        private static bool TryGetTargetConstructor(this List<ConstructorsStruct> markersStorages, out ConstructorsStruct targetConstructor)
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

            ParameterInfo[] parameters = targetConstructor.MemberInfo.GetParameters();
            targetConstructor.MarkerAttribute.Initialization(targetConstructor.MemberInfo, parameters);

            return targetConstructor.MarkerAttribute != null;
        }

        //  Google Sheet
        public static bool CheckRowsForData(this List<string> rows)
        {
            for (int i = 0; i < rows.Count; i++)
                if (!String.IsNullOrWhiteSpace(rows[i]))
                    return true;

            return false;
        }

        private static bool CheckStartComplexString(this string checkString) => checkString.Length > 0 && checkString[0] == '"';
        private static bool CheckEndComplexString(this string checkString) => checkString.Length > 0 && checkString[checkString.Length - 1] == '"';

        public static void ParseStringToColumns(this string line, out List<string> outColumns)
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

        public static void OutputMemberDictionary<TAttribute, TMember>(Dictionary<TMember, TAttribute> pairs, string keyTitle, string valueTitle, bool isShow = false)
                where TAttribute : System.Attribute
                where TMember : MemberInfo
        {
            for (int i = 0; i < pairs.Count; i++)
                ConverterLog.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}", isShow);
        }

        public static void OutputDictionary<TKey, TValue>(Dictionary<TValue, TKey> pairs, string keyTitle, string valueTitle, bool isShow = false)
        {
            for (int i = 0; i < pairs.Count; i++)
                ConverterLog.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}", isShow);
        }

        public static void OutputMarkersStruct<TAttribute, TMember>(List<MarkersStorage<TAttribute, TMember>> markers, string memberTitle, string ttributeTitle, bool isShow = false)
            where TAttribute : LoadMarkerAttribute
            where TMember : MemberInfo
        {
            for (int i = 0; i < markers.Count; i++)
                ConverterLog.Log($"{markers[i].MarkerAttribute.Column} | {memberTitle}: {markers[i].MemberInfo} / {ttributeTitle}: {markers[i].MarkerAttribute}", isShow);
        }

        public static void OutputConstructorStruct(ConstructorsStruct marker, bool isShow = false)
        {
            ConverterLog.Log($"Constructor: {marker.MemberInfo} / {marker.MarkerAttribute.Columns}", isShow);
            ParameterInfo[] parameters = marker.MemberInfo.GetParameters();

            for (int p = 0; p < parameters.Length; p++)
                ConverterLog.Log($"Param {parameters[p].Position} is named {parameters[p].Name} and is of type {parameters[p].ParameterType}", isShow);
        }

        public static void OutputConstructorsStruct(List<ConstructorsStruct> markers, bool isShow = false)
        {
            for (int m = 0; m < markers.Count; m++)
            {
                ConverterLog.Log($"Constructor: {markers[m].MemberInfo} / {markers[m].MarkerAttribute.Columns}", isShow);
                ParameterInfo[] parameters = markers[m].MemberInfo.GetParameters();

                for (int p = 0; p < parameters.Length; p++)
                    ConverterLog.Log($"Param {parameters[p].Position} is named {parameters[p].Name} and is of type {parameters[p].ParameterType}", isShow);
            }
        }

        #endregion
    }
}
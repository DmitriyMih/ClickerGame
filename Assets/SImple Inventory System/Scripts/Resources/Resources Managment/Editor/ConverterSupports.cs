using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
    private static bool TryGetCustomAttribute<TAttribute>(this MemberInfo field, out TAttribute attribute) where TAttribute : System.Attribute
    {
        attribute = field.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    public static bool TryGetCustomAttributes<TMember, TAttribute>(TMember[] memberInfos, out List<MarkersStorage<TAttribute, TMember>> markersStorage)
        where TAttribute : BaseMarkerAttribute
        where TMember : MemberInfo
    {
        markersStorage = new();

        for (int m = 0; m < memberInfos.Length; m++)
            if (memberInfos[m].TryGetCustomAttribute(out TAttribute marker))
                markersStorage.Add(new(marker, memberInfos[m]));

        return markersStorage.Count > 0;
    }

    public static void OutputDictionary<TAttribute, TMember>(Dictionary<TMember, TAttribute> pairs, string keyTitle, string valueTitle)
        where TAttribute : System.Attribute
        where TMember : MemberInfo
    {
        for (int i = 0; i < pairs.Count; i++)
            Debug.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}");
    }

    public static void OutputMarkersStruct<TAttribute, TMember>(List<MarkersStorage<TAttribute, TMember>> markers, string memberTitle, string ttributeTitle)
        where TAttribute : BaseMarkerAttribute
        where TMember : MemberInfo
    {
        for (int i = 0; i < markers.Count; i++)
            Debug.Log($"{markers[i].MarkerAttribute.Column} | {memberTitle}: {markers[i].MemberInfo} / {ttributeTitle}: {markers[i].MarkerAttribute}");
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ConverterSupports
{
    private static bool TryGetCustomAttribute<TAttribute>(this MemberInfo field, out TAttribute attribute) where TAttribute : System.Attribute
    {
        attribute = field.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    public static bool TryGetCustomAttributes<TMember, TAttribute>(this System.Type type, TMember[] memberInfos, out Dictionary<TMember, TAttribute> outDictionary)
        where TAttribute : System.Attribute
        where TMember : MemberInfo
    {
        outDictionary = new();

        for (int m = 0; m < memberInfos.Length; m++)
            if (memberInfos[m].TryGetCustomAttribute(out TAttribute marker))
                outDictionary.Add(memberInfos[m], marker);

        return outDictionary.Count > 0;
    }

    public static void OutputDictionary<TAttribute, TMember>(this System.Type type, Dictionary<TMember, TAttribute> pairs, string keyTitle, string valueTitle)
        where TAttribute : System.Attribute
        where TMember : MemberInfo
    {
        for (int i = 0; i < pairs.Count; i++)
            Debug.Log($"{i} | {keyTitle}: {pairs.ElementAt(i).Key} / {valueTitle}: {pairs.ElementAt(i).Value}");
    }
}
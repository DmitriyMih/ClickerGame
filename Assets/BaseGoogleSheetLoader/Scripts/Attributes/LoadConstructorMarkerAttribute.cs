using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class LoadConstructorMarkerAttribute : Attribute
{
    public ConstructorInfo ConstructorInfo { get; private set; }
    public int[] Columns { get; private set; }

    public ParameterInfo[] ArgumentsInfos { get; private set; }

    public LoadConstructorMarkerAttribute(params int[] columns) { Columns = columns; }

    public void Initialization(ConstructorInfo constructorInfo, ParameterInfo[] argumentsInfos)
    {
        ConstructorInfo = constructorInfo;
        ArgumentsInfos = argumentsInfos;

        UnityEngine.Debug.Log($"Constructor Info: {constructorInfo}");

        for (int i = 0; i < argumentsInfos.Length; i++)
            UnityEngine.Debug.Log($"Argument Info {i}: {argumentsInfos[i]}");
    }
}
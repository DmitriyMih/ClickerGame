using GoogleSheetLoaderSystem;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class LoadConstructorMarkerAttribute : Attribute
{
    public ConstructorInfo ConstructorInfo { get; private set; }
    public int[] Columns { get; private set; }

    public ParameterInfo[] ArgumentsInfos { get; private set; }

    public LoadConstructorMarkerAttribute(params int[] columns) { Columns = columns; }

    public void Initialization(ConstructorInfo constructorInfo, ParameterInfo[] argumentsInfos, bool isShowProcess)
    {
        ConstructorInfo = constructorInfo;
        ArgumentsInfos = argumentsInfos;

        SupportLog.Log($"Constructor Info: {constructorInfo}", isShowProcess);

        for (int i = 0; i < argumentsInfos.Length; i++)
            SupportLog.Log($"Argument Info {i}: {argumentsInfos[i]}", isShowProcess);
    }
}
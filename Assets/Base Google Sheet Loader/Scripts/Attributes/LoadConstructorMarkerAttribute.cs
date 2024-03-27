using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class LoadConstructorMarkerAttribute : Attribute
{
    public ConstructorInfo ConstructorInfo { get; private set; }
    public int[] Columns { get; private set; }
    public object[] ConstructorArguments;

    public LoadConstructorMarkerAttribute(params int[] columns) { Columns = columns; }

    public void Initialization(ConstructorInfo constructorInfo, object[] constructorArguments)
    {
        ConstructorInfo = constructorInfo;
        ConstructorArguments = constructorArguments;
    }
}
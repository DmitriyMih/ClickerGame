using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class LoadMarkerAttribute : Attribute
{
    public FieldInfo FieldInfo { get; private set; }
    public int Column { get; private set; }

    public Type FieldArgument { get; private set; }

    public LoadMarkerAttribute(int column) => Column = column;

    public void Initialization(FieldInfo fieldInfo, Type argumentType)
    {
        FieldInfo = fieldInfo;
        FieldArgument = argumentType;
    }
}
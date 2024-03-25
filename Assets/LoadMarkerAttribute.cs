using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class LoadMarkerAttribute : Attribute
{
    public int Column { get; private set; }
    public LoadMarkerAttribute(int column) => Column = column;
}
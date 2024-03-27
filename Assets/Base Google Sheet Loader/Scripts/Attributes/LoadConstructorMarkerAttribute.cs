using System;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class LoadConstructorMarkerAttribute : Attribute
{
    public int[] Columns { get; private set; }
    public LoadConstructorMarkerAttribute(params int[] columns) { Columns = columns; }
}
using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class LoadMarkerAttribute : BaseMarkerAttribute
{
    public int Column { get; private set; }
    public LoadMarkerAttribute(int column) => Column = column;
}
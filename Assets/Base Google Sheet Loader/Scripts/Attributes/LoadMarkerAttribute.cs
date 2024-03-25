using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class LoadMarkerAttribute : BaseMarkerAttribute
{
    public LoadMarkerAttribute(int column) : base(column) { }
}
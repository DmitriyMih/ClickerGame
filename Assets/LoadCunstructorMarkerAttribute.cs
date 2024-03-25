using System;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class LoadCunstructorMarkerAttribute: Attribute
{
    public int Column { get; private set; }
    public LoadCunstructorMarkerAttribute(int column) => Column = column;
}
using System;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
public class LoadCunstructorMarkerAttribute : BaseMarkerAttribute
{
    public LoadCunstructorMarkerAttribute(int column) : base(column) { }
}
using System;

public abstract class BaseMarkerAttribute : Attribute
{
    public int Column { get; private set; }
    public BaseMarkerAttribute(int column) => Column = column;
}
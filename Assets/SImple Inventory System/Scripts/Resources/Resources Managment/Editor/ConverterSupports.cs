using System.Reflection;

public static class ConverterSupports
{
    public static bool TryGetCustomAttribute<TAttribute>(this MemberInfo memberInfo, FieldInfo field, out TAttribute attribute) where TAttribute : System.Attribute
    {
        attribute = field.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }
}
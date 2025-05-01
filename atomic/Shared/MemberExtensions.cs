namespace System.Reflection;

[DebuggerStepThrough]
internal static class MemberExtensions
{
    public static bool IsDefined<TAttribute>(this Assembly assembly) where TAttribute : Attribute
    {
        return assembly.IsDefined(typeof(TAttribute));
    }

    public static bool IsDefined<TAttribute>(this MemberInfo memberInfo) where TAttribute : Attribute
    {
        return memberInfo.IsDefined(typeof(TAttribute));
    }

    public static string GetSimpleMemberName(this MemberInfo member)
    {
        var name = member.Name;
        var index = name.LastIndexOf('.');
        return index >= 0 ? name[(index + 1)..] : name;
    }

    public static bool IsReallyVirtual(this MethodInfo method)
        => method is { IsVirtual: true, IsFinal: false };
}

namespace Nop.Plugin.Widgets.Employees.Extensions;

public static class StringExtensions
{
    public static string NoAsync(this string s)
        => s.EndsWith("Async") ? s[0..^5] : s;

    public static string NoController(this string s) => s.Replace("Controller", "");
}
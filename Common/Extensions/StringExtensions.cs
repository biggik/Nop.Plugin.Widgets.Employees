namespace Nop.Plugin.Widgets.Employees.Extensions;

public static class StringExtensions
{
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    public static string NoAsync(this string s) => s.Replace("Async", "");
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods

    public static string NoController(this string s) => s.Replace("Controller", "");
}
namespace ITracker.Core.Domain;

internal static class StringExtensions
{
	public static string CleanHtmlValue(this string? value)
	{
		return value?
		.Replace("&nbsp;", " ")
		.Trim() ?? string.Empty;
	}

	public static string CleanHtmlNumber(this string? value)
	{
		return value?.CleanHtmlValue().Replace(",", ".").Replace(" ", string.Empty) ?? string.Empty;
	}
}


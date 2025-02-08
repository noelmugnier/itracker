namespace ITracker.Core.Domain;

public record PropertySelector
{
	private static string _selectorPattern = @"^(?<CssSelector>.*)(\=\>(?<Attribute>\[(?<AttributeValue>[A-z0-9-]+)(\:\:(?<AttributeRegex>.*))?\]))$";

	private Match? _matchedResult = null;
	private PropertyAttributeSelector? _propertyAttributeSelector = null;
	private PropertyValueSource? _source = null;
	private string? _cssSelector = null;

	private PropertySelector() { }

	public PropertySelector(string value)
	{
		if (value is null)
			throw new DomainException(ErrorCode.PropertySelectorIsRequired);

		if (string.IsNullOrWhiteSpace(value))
			throw new DomainException(ErrorCode.PropertySelectorCannotBeEmpty);

		Value = value;
	}

	public string Value { get; }

	public string GetCssSelector()
	{
		if (_cssSelector == null)
		{
			_matchedResult = _matchedResult ?? Regex.Match(Value, _selectorPattern);
			if (!_matchedResult.Success)
				_cssSelector = Value;
			else
				_cssSelector = _matchedResult.Groups["CssSelector"].Value;
		}

		return _cssSelector;
	}

	public PropertyValueSource GetSource()
	{
		if (_source == null)
		{
			_matchedResult = _matchedResult ?? Regex.Match(Value, _selectorPattern);
			if (!_matchedResult.Success)
				_source = PropertyValueSource.InnerHtml;
			else
				_source = _matchedResult.Groups["Attribute"].Success
					? PropertyValueSource.Attribute
					: PropertyValueSource.InnerHtml;
		}

		return _source.Value;
	}

	public PropertyAttributeSelector GetAttribute()
	{
		if (_propertyAttributeSelector == null)
		{
			_matchedResult = _matchedResult ?? Regex.Match(Value, _selectorPattern);
			if (GetSource() != PropertyValueSource.Attribute)
				throw new InvalidOperationException(ErrorCode.PropertySelectorIsNotAttributeSelector.ToString("G"));

			var attributeValue = _matchedResult.Groups["AttributeValue"].Value;
			var attributeRegex = _matchedResult.Groups["AttributeRegex"].Value;

			_propertyAttributeSelector = new PropertyAttributeSelector(attributeValue, attributeRegex);
		}

		return _propertyAttributeSelector;
	}

	public static PropertySelector From(string value)
	{
		return new PropertySelector(value);
	}

	internal static PropertySelector Empty()
	{
		return new PropertySelector();
	}
}
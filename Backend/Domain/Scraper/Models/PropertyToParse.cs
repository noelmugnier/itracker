namespace ITracker.Core.Domain;

public abstract record PropertyToParse
{
	protected PropertyToParse() { }
	protected PropertyToParse(PropertyName name, PropertySelector valueSelector, ValueKind valueType, bool required = true)
	{
		Name = name;
		ValueSelector = valueSelector;
		ValueType = valueType;
		Required = required;
	}

	public PropertyName Name { get; }
	public PropertySelector ValueSelector { get; }
	public ValueKind ValueType { get; }
	public bool Required { get; }

	public abstract Result<PropertyParsed> ParseAttributeValue(string? attributeValue);
	public abstract Result<PropertyParsed> ParseNodeValue(string rawValue);

	public static PropertyToParse Create(PropertyName name, PropertySelector selector, ValueKind valueType, bool required = true)
	{
		return valueType switch {
			ValueKind.Text => new TextToParse(name, selector, required),
			ValueKind.Integer => new IntegerToParse(name, selector, required),
			ValueKind.Money => new MoneyToParse(name, selector, required),
			ValueKind.Decimal => new DecimalToParse(name, selector, required),
			ValueKind.Percentage => new PercentageToParse(name, selector, required),
			ValueKind.Capacity => new CapacityToParse(name, selector, required),
			ValueKind.Date => new DateToParse(name, selector, required),
			ValueKind.Time => new TimeToParse(name, selector, required),
			ValueKind.Url => new UrlToParse(name, selector, required),
			ValueKind.Rating => new RatingToParse(name, selector, required),
			ValueKind.Email => new EmailToParse(name, selector, required),
			ValueKind.Phone => new PhoneToParse(name, selector, required),
			_ => throw new InvalidOperationException($"Value type {valueType} is not supported")
		};		
	}
}

public abstract record PropertyToParse<T> : PropertyToParse where T : PropertyParsed
{
	private PropertyToParse(){}
	protected PropertyToParse(PropertyName name, PropertySelector selector, ValueKind valueType, bool required = true)
		: base(name, selector, valueType, required)
	{
	}

	public abstract T? Parse(string? value);

	public override Result<PropertyParsed> ParseAttributeValue(string? attributeValue)
	{
		if (ValueSelector.GetSource() != PropertyValueSource.Attribute)
			throw new InvalidOperationException($"Property {Name} is not an attribute property");

		var propertyRawValue = string.Empty;
		if (string.IsNullOrWhiteSpace(ValueSelector.GetAttribute().Regex))
			propertyRawValue = attributeValue;
		else
		{
			var regex = new Regex(ValueSelector.GetAttribute().Regex!);
			var matches = regex.Match(attributeValue ?? string.Empty);
			if (matches.Success && matches.Groups[Name.Value].Success)
				propertyRawValue = matches.Groups[Name.Value].Value;
			else if (matches.Success)
				propertyRawValue = matches.Value;
		}

		return ParseValue(propertyRawValue);
	}

	public override Result<PropertyParsed> ParseNodeValue(string rawValue)
	{
		if (ValueSelector.GetSource() != PropertyValueSource.InnerHtml)
			throw new InvalidOperationException($"Property {Name} is not an node property");

		return ParseValue(rawValue);
	}

	private Result<PropertyParsed> ParseValue(string? rawValue)
	{
		rawValue = rawValue?.Trim();
		var parsedValue = Parse(rawValue);
		if (parsedValue == null)
			return Result.Fail(new DomainError(ErrorCode.PropertyParserRequiredPropertyParseFailed, $"Property {Name} is required and has failed to parse value: {rawValue}"));

		return parsedValue;
	}
}

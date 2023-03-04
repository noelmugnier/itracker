namespace ITracker.Core.Domain;

public abstract record PropertyParsed
{
	protected PropertyParsed() { }

	protected PropertyParsed(PropertyName name, ValueKind valueType)
	{
		Name = name;
		ValueType = valueType;
	}

	public PropertyName Name { get; }
	public ValueKind ValueType { get; }
	public abstract object? RawValue {get;}

  public PropertyParsed AsNullProperty()
  {

return ValueType switch {
			ValueKind.Text => new TextParsed(Name, null),
			ValueKind.Integer => new IntegerParsed(Name, null),
			ValueKind.Money => new MoneyParsed(Name, null),
			ValueKind.Decimal => new DecimalParsed(Name, null),
			ValueKind.Percentage => new PercentageParsed(Name, null),
			ValueKind.Capacity => new CapacityParsed(Name, null),
			ValueKind.Date => new DateParsed(Name, null),
			ValueKind.Time => new TimeParsed(Name, null),
			ValueKind.Url => new UrlParsed(Name, null),
			ValueKind.Rating => new RatingParsed(Name, null),
			ValueKind.Email => new EmailParsed(Name, null),
			ValueKind.Phone => new PhoneParsed(Name, null),
			_ => throw new InvalidOperationException($"Value type {ValueType} is not supported")
		};		
  }
}

public abstract record PropertyParsed<T, U> : PropertyParsed 
	where T : ValueObject<U>
{
	private PropertyParsed() { }

	protected PropertyParsed(PropertyName name, ValueKind valueType, T? parsedValue)
		: base(name, valueType)
	{
		ParsedValue = parsedValue;
	}

	public T? ParsedValue { get; }
	
	public override object? RawValue {
		get {
			if (ParsedValue == null)
				return null;

			return ParsedValue.Value;
		}
	}
}

namespace ITracker.Core.Domain;

public record ParsedElement
{
	private readonly List<PropertyParsed> _properties = new();
	public IReadOnlyCollection<PropertyParsed> Properties => _properties;

	private ParsedElement() { }

	public ParsedElement(IEnumerable<PropertyParsed> properties)
	{
		_properties = properties.ToList();
	}

	public Guid Id { get; } = Guid.NewGuid();

	public T? GetProperty<T>(PropertyName name) where T : PropertyParsed
	{
		var value = _properties.SingleOrDefault(p => p.Name == name);
		return value as T;
	}
}


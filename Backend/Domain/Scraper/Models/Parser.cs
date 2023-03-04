namespace ITracker.Core.Domain;

public record Parser
{
	private readonly List<PropertyToParse> _properties = new List<PropertyToParse>();

	private Parser() { }

	internal Parser(ElementSelector elementSelector, IEnumerable<PropertyToParse> properties)
	{
		ElementSelector = elementSelector;
		_properties = properties?.ToList() ?? new List<PropertyToParse>();
	}

	public ElementSelector ElementSelector { get; }
	public IReadOnlyCollection<PropertyToParse> Properties => _properties.AsReadOnly();
}

public class ParserBuilder 
{
	private ElementSelector _elementSelector;
	private List<PropertyToParse> _properties = new List<PropertyToParse>();

	public ParserBuilder WithElementSelector(string elementSelector)
	{
		_elementSelector = ElementSelector.From(elementSelector);
		return this;
	}

	public ParserBuilder WithProperties(IEnumerable<PropertyToParse> properties)
	{
		_properties.AddRange(properties);
		return this;
	}

	public ParserBuilder AddProperty(string name, string selector, ValueKind valueKind, bool required = true)
	{
		_properties.Add(PropertyToParse.Create(PropertyName.From(name), PropertySelector.From(selector), valueKind, required));
		return this;
	}

	public Parser Build()
	{
		return new Parser(_elementSelector, _properties);
	}
}


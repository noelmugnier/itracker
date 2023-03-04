namespace ITracker.Core.Domain;

public record ProductPropertySchema(PropertyName PropertyName, Name DisplayName, ValueKind ValueType, bool Required = true, bool Tracked = false);
public record ProductIdPropertySchema(Name DisplayName) : ProductPropertySchema(PropertyName.Identifier, DisplayName, ValueKind.Text, true, false);
public record ProductNamePropertySchema(Name DisplayName) : ProductPropertySchema(PropertyName.DisplayName, DisplayName, ValueKind.Text, true, false);
public record ProductPricePropertySchema(Name DisplayName) : ProductPropertySchema(PropertyName.Price, DisplayName, ValueKind.Money, true, true);

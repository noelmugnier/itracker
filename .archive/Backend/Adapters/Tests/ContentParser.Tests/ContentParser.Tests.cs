using ITracker.Core.Domain;

namespace ITracker.Adapters.ContentParser.Tests;

public class ContentParserTests
{
    [Fact]
    public void Should_ReturnOneStringElement_When_ValidContentAndSelectorProvided()
    {        
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
          </section>";

        var parser = new ParserBuilder()
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {  
                result.Value.Elements.Should().HaveCount(1);
                
                var property = result.Value.Elements.Single().GetProperty<TextParsed>(PropertyName.From(PropertyName.DisplayName.Value))!.ParsedValue!;
                property.Should().NotBeNull();
                property.Value!.Should().Be("Product1");
            });
    }

    [Fact]
    public void Should_ReturnOneStringElementTrimmed_When_ValidContentAndSelectorProvided()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>  Product1 test  </h5><div>12.5</div></article>
          </section>";

        var parser = new ParserBuilder()
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(1);
                
                var property = result.Value.Elements.Single().GetProperty<TextParsed>(PropertyName.From(PropertyName.DisplayName.Value))!.ParsedValue!;
                property.Should().NotBeNull();
                property.Value!.Should().Be("Product1 test");
            });
    }

    [Fact]
    public void Should_ReturnMatchedElements_When_ValidContentAndSelectorProvided()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
            <article><h1>p3</h1><h5>Product3</h5><div>30,99</div></article>
          </section>";

        var parser = new ParserBuilder()
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty("value", "div", ValueKind.Decimal)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(2);
            });
    }

    [Fact]
    public void Should_ReturnCapacity()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1 test</h5><div class='capacity'>0,75 L</div></article>
          </section>";

		    var name = PropertyName.From("capacity");

        var parser = new ParserBuilder()
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty(name.Value, "div.capacity", ValueKind.Capacity)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(1);

                var property = result.Value.Elements.Single().GetProperty<CapacityParsed>(name);
                property.Should().NotBeNull();

                property!.ParsedValue!.Value.Should().Be(0.75m);
                property!.ParsedValue!.Unit.Should().Be("L");
            });
    }

    [Fact]
    public void Should_ReturnPercent()
    {        
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1 test</h5><div class='degree'>14,5%</div></article>
          </section>";

		    var name = PropertyName.From("degree");        

        var parser = new ParserBuilder()
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty(name.Value, "div.degree", ValueKind.Percentage)
            .Build();        

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(1);

                var property = result.Value.Elements.Single().GetProperty<PercentageParsed>(name);
                property.Should().NotBeNull();

                property!.ParsedValue!.Value.Should().Be(0.145m);
            });
    }

    [Fact]
    public void Should_ReturnPrice()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1 test</h5><div class='price'>14,9 €</div></article>
          </section>";

		    var name = PropertyName.From("degree");   

        var parser = new ParserBuilder()           
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty(name.Value, "div.price", ValueKind.Money)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(1);

                var property = result.Value.Elements.Single().GetProperty<MoneyParsed>(name);
                property.Should().NotBeNull();

                property!.ParsedValue!.Value.Should().Be(14.90m);
                property!.ParsedValue!.Currency.Should().Be("€");
            });
    }

    [Fact]
    public void Should_ReturnMultipleProducts()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1 test</h5><div>14,9</div><span>Bio</span></article>
            <article><h1>p2</h1><h5>Product2 test</h5><div>14,9</div><span>Non Bio</span></article>
          </section>";

		    var name = PropertyName.From("label");           

        var parser = new ParserBuilder()          
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty(name.Value, "span", ValueKind.Text)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(2);
                
                result.Value.Elements.First(v => v.GetProperty<TextParsed>(name)!.ParsedValue!.Value == "Bio").Should().NotBeNull();
                result.Value.Elements.First(v => v.GetProperty<TextParsed>(name)!.ParsedValue!.Value == "Non Bio").Should().NotBeNull();
            });
    }

    [Fact]
    public void Should_ReturnProductIdFromAttribute()
    {   
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div><button id='53791'>ok</button></article>
          </section>";

        var parser = new ParserBuilder()         
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "button=>[id]", ValueKind.Integer)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(1);
                
                var property = result.Value.Elements.Single().GetProperty<IntegerParsed>(PropertyName.Identifier)!.ParsedValue!;
                property.Should().NotBeNull();
                property.Value.Should().Be(53791);
            });
    }

    [Fact]
    public void Should_ReturnProductIdFromPartOfAttributeUsingRegex()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div><a href='/53791-super-product'>visit</a></article>
          </section>";

        var parser = new ParserBuilder()        
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, @"a=>[href::^\/(?<id>[0-9]+).*]", ValueKind.Integer)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should()
            .BeSuccess()
            .And.Satisfy(result =>
            {
                result.Value.Elements.Should().HaveCount(1);
                
                var property = result.Value.Elements.Single().GetProperty<IntegerParsed>(PropertyName.From(PropertyName.Identifier.Value))!.ParsedValue!;
                property.Should().NotBeNull();
                property.Value.Should().Be(53791);
            });
    }

    [Fact]
    public void Should_ReturnErrorIfRequiredAndNotFound()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div><a href='/53791-super-product'>visit</a></article>
          </section>";             

        var parser = new ParserBuilder()       
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, @".vue-produ:first-child a=>[href::^\/(?<id>[0-9]+).*]", ValueKind.Integer)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should().BeSuccess();
        parsedElements.Value.Errors.First().As<DomainError>().Code.Should().Be(ErrorCode.HtmlPropertyParserRequiredPropertyNotFound);
    }

    [Fact]
    public void Should_ReturnErrorIfRequiredAndInvalidValue()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div><span>test</span></article>
          </section>"; 

        var parser = new ParserBuilder()      
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty("label", "span", ValueKind.Decimal)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);
        
        parsedElements.Should().BeSuccess();
        parsedElements.Value.Errors.First().As<DomainError>().Code.Should().Be(ErrorCode.PropertyParserRequiredPropertyParseFailed);
    }

    [Fact]
    public void Should_ReturnErrorIfNotRequiredAndInvalidValue()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div><span>test</span></article>
          </section>";

        var parser = new ParserBuilder()     
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty("label", "span", ValueKind.Decimal)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should().BeSuccess();
        parsedElements.Value.Errors.First().As<DomainError>().Code.Should().Be(ErrorCode.PropertyParserRequiredPropertyParseFailed);
    }

    [Fact]
    public void Should_ReturnNoErrorIfNotRequiredAndNotFound()
    {
        var content = @"
          <section>
            <article><h1>p1</h1><h5>Product1</h5><div>12.5</div></article>
          </section>";

        var parser = new ParserBuilder()    
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty("label", "span", ValueKind.Decimal, false)
            .Build();

        var parsedElements = new AngleSharpParserAdapter().Parse(content, parser);

        parsedElements.Should().BeSuccess();
        parsedElements.Value.Elements.Should().HaveCount(1);
        parsedElements.Value.Elements.First().Properties.Should().HaveCount(2);
    }
}

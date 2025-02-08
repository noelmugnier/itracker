namespace ITracker.Core.Application.Tests;

public class CreateDefaultBrandTests
{
    [Fact]
    public async void Should_CreateDefaultBrand()
    {
        var uow = new FakeUnitOfWork();
        var command = new CreateDefaultBrandCommand("Test Brand");

        var result = await new CreateDefaultBrandCommandHandler(uow).Handle(command, CancellationToken.None);

        result.Should().BeSuccess();
        var brand = (await uow.Get<IBrandRepository>().Get(BrandId.From(result.Value), CancellationToken.None)).AsT0;
        brand.Should().NotBeNull();
        brand.IsDefault.Should().BeTrue();
    }
}

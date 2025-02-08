namespace ITracker.Core.Application.Tests;

public class CreateBrandTests
{
	[Fact]
	public async void Should_CreateBrand()
	{
        var uow = new FakeUnitOfWork();
		var command = new CreateBrandCommand("Test Brand");

		var result = await new CreateBrandCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var brand = (await uow.Get<IBrandRepository>().Get(BrandId.From(result.Value), CancellationToken.None)).AsT0;
		brand.Should().NotBeNull();
	}
}
namespace ITracker.Core.Application.Tests;

public class DeleteBrandTests
{
	[Fact]
	public async void Should_DeleteBrand()
	{
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .Build();	
		var command = new DeleteBrandCommand(brandId.Value);

		var result = await new DeleteBrandCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var findResult = await uow.Get<IBrandRepository>().Get(brandId, CancellationToken.None);
		findResult.IsT1.Should().BeTrue();
	}
}
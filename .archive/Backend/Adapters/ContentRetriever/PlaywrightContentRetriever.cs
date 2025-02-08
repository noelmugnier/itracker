using Microsoft.Playwright;
using ITracker.Core.Domain;
using FluentResults;

namespace ITracker.Adapters.ContentRetriever;

public class PlaywrightContentRetrieverAdapter : IContentRetriever
{
	public async Task<Result<string>> Retrieve(Uri uri, CancellationToken token)
	{
		try
		{
			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
			var page = await browser.NewPageAsync(new BrowserNewPageOptions { IgnoreHTTPSErrors = true });
			var response = await page.GotoAsync(uri.ToString(), new() { WaitUntil = WaitUntilState.NetworkIdle });
			if (response == null || !response.Ok)
			{
				await page.CloseAsync();
				return Result.Fail(response?.StatusText ?? "Unknown error occured while retrieving page content");
			}

			await response.FinishedAsync();
			var content = await page.ContentAsync();
			await page.CloseAsync();
			if (string.IsNullOrWhiteSpace(content))
				return Result.Fail("Page content is empty");

			return content;
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}
}
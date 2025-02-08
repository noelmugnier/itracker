import { Option, Some, None } from "ts-results-es";
import { CustomError, createError, IRemoveBrand, BrandId } from "@itracker/domain";

export class RemoveBrandHttpAdapter implements IRemoveBrand {
	private readonly _fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>;
	private readonly _baseUrl: URL;

	constructor(fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>, baseUrl: URL) {
		this._fetch = fetch;
		this._baseUrl = baseUrl;
	}

	async remove(brandId: BrandId): Promise<Option<CustomError>> {
		try {
			const res = await this._fetch(
				`${this._baseUrl.toString()}api/brands/${brandId}`,
				{
					method: "DELETE"
				});

			if (!res.ok)
				return Some(createError(res.statusText, res.status));

			return None;
		}
		catch (err: any) {
			return Some(createError(err.toString()));
		}
	}
}
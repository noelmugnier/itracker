import { Ok, Err, type Result } from "ts-results-es";
import { CustomError, createError, BrandId, Brand, IRetrieveBrand } from "@itracker/domain";

export class RetrieveBrandHttpAdapter implements IRetrieveBrand {
	private readonly _fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>;
	private readonly _baseUrl: URL;

	constructor(fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>, baseUrl: URL) {
		this._fetch = fetch;
		this._baseUrl = baseUrl;
	}

	get = async (brandId: BrandId): Promise<Result<Brand, CustomError>> => {
		try {
			const res = await this._fetch(`${this._baseUrl.toString()}api/brands/${brandId}`);
			if (!res.ok)
				return Err(createError(res.statusText, res.status));

			const data = await res.json();
			return Ok(data.brand);
		}
		catch (err: any) {
			return Err(createError(err.toString()));
		}
	}
}
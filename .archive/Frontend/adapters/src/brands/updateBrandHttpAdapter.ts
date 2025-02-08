import { type Option, Some, None } from "ts-results-es";
import { IUpdateBrand, CustomError, createError, Brand } from "@itracker/domain";

export class UpdateBrandHttpAdapter implements IUpdateBrand {
	private readonly _fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>;
	private readonly _baseUrl: URL;

	constructor(fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>, baseUrl: URL) {
		this._fetch = fetch;
		this._baseUrl = baseUrl;
	}

	async save(brand: Brand): Promise<Option<CustomError>> {
		try {
			const res = await this._fetch(
				`${this._baseUrl.toString()}api/brands/${brand.id}`,
				{
					method: "PUT",
					headers: {
						"Content-Type": "application/json"
					},
					body: JSON.stringify(brand)
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
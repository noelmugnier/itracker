import { Ok, Err, type Result } from "ts-results-es";
import { ICreateBrand, CreateBrand, CustomError, createError, BrandId } from "@itracker/domain";

export class CreateBrandHttpAdapter implements ICreateBrand {
	private readonly _fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>;
	private readonly _baseUrl: URL;

	constructor(fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>, baseUrl: URL) {
		this._fetch = fetch;
		this._baseUrl = baseUrl;
	}

	async create(brand: CreateBrand): Promise<Result<BrandId, CustomError>> {
		try {
			const res = await this._fetch(
				`${this._baseUrl.toString()}api/brands`,
				{
					method: "POST",
					headers: {
						"Content-Type": "application/json"
					},
					body: JSON.stringify({ ...brand, isDefault: false })
				});

			if (!res.ok)
				return Err(createError(res.statusText, res.status));

			const data = await res.json();
			return Ok(data);
		}
		catch (err: any) {
			return Err(createError(err.toString()));
		}
	}
}
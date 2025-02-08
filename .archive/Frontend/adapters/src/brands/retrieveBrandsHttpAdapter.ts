import { Ok, Err, type Result } from "ts-results-es";
import { PageNumber, PageSize, CustomError, createError, IRetrieveBrands, PaginatedBrands } from "@itracker/domain";

export class RetrieveBrandsHttpAdapter implements IRetrieveBrands {
	private readonly _fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>;
	private readonly _baseUrl: URL;

	constructor(fetch: (input: RequestInfo, init?: RequestInit | undefined) => Promise<Response>, baseUrl: URL) {
		this._fetch = fetch;
		this._baseUrl = baseUrl;
	}

	async get(page: PageNumber, count: PageSize): Promise<Result<PaginatedBrands, CustomError>> {
		try {
			const res = await this._fetch(`${this._baseUrl.toString()}api/brands?PageNumber=${page}&PageSize=${count}`);
			if (!res.ok)
				return Err(createError(res.statusText, res.status));

			const data = await res.json();
			return Ok({...data, pageNumber: page, pageSize: count});
		}
		catch (err: any) {
			return Err(createError(err.toString()));
		}
	}
}
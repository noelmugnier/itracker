import type { IHttpClient } from "@itracker/adapters";

export class HttpClient implements IHttpClient {
	constructor(private _fetch: typeof global.fetch, private _baseUrl:string){}

	async get<T>(url: string): Promise<T> {
		const res = await this._fetch(`${this._baseUrl}/${url}`);
		return await res.json();
	}
}
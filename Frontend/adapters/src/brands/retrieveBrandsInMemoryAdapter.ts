import { Brand, CustomError, IRetrieveBrands, PaginatedBrands } from "@itracker/domain";
import { Ok, Result } from "ts-results-es";

export class RetrieveBrandsInMemoryAdapter implements IRetrieveBrands {
	private _brands: Brand[] = [];

	initWithBrands(brands: Brand[]): void {
		this._brands = brands;
	}

	get(pageNumber: number, pageSize: number): Promise<Result<PaginatedBrands, CustomError>> {
		return Promise.resolve<Result<PaginatedBrands, CustomError>>(Ok({
			brands: this._brands.slice((pageNumber - 1) * pageSize, pageNumber * pageSize),
			pageNumber: pageNumber,
			pageSize: pageSize,
			totalBrands: this._brands.length
		}));
	}
}

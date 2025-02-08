import { Result, Option } from "ts-results-es";
import { CustomError, PageNumber, PageSize } from "../../shared";
import { Brand, BrandId, CreateBrand, PaginatedBrands } from "./models";

export interface ICreateBrand {
	create(brandToCreate: CreateBrand): Promise<Result<BrandId, CustomError>>;
}

export interface IRetrieveBrand {
	get(brandId: BrandId): Promise<Result<Brand, CustomError>>
}

export interface IRetrieveBrands {
	get(page: PageNumber, size: PageSize): Promise<Result<PaginatedBrands, CustomError>>
}

export interface IRemoveBrand{
	remove(brandId: string): Promise<Option<CustomError>>;
}

export interface IUpdateBrand {
	save(brand: Brand): Promise<Option<CustomError>>;
}
import { PageNumber, PageSize } from "../../shared";

export type Brand = {
	id: BrandId;
	name: string;
	isDefault: boolean;
}

export type CreateBrand = {
	name: string;
}

export type BrandId = string;

export type PaginatedBrands = {
	brands: Brand[];
	pageNumber: PageNumber;
	pageSize: PageSize;
	totalBrands: number;
}
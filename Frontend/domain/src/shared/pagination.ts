import { PageNumber, parsePageNumber } from "./pageNumber";
import { PageSize, parsePageSize } from "./pageSize";

export type Pagination = {
	readonly pageNumber: PageNumber;
	readonly pageSize: PageSize;
	nextPage(): Pagination;
	previousPage(): Pagination;
	changePageSize(newPageCount: PageSize): Pagination;
}

export const createPagination = (pageNumber?: PageNumber | undefined, pageSize?: PageSize | undefined): Pagination => {	
	const getValidPageNumber = (page: PageNumber | undefined):PageNumber => {
		page = page ?? 1;
		page = page <= 0 ? 1 : page;
		return page;
	}

	const getValidPageSize = (page: PageNumber | undefined):PageSize => {
		page = page ?? 10;
		page = page <= 0 ? 10 : page;
		return page;
	}
	
	return {
		pageNumber : getValidPageNumber(pageNumber),
		pageSize: getValidPageSize(pageSize),
		nextPage: () => createPagination(getValidPageNumber(pageNumber) + 1, pageSize),
		previousPage: () => {
			let currentPage = getValidPageNumber(pageNumber);
			return createPagination(--currentPage <= 0 ? 1 : currentPage, pageSize);
		},
		changePageSize: (newPageSize: PageSize) => createPagination(1, getValidPageSize(newPageSize))
	}
}

export const parseFromSearchParams = (url: URL, pageNumberParamName: string, pageSizeParamName: string): Pagination => {
	let pageNumber = parsePageNumber(url.searchParams.get(pageNumberParamName));
	let pageCount = parsePageSize(url.searchParams.get(pageSizeParamName));

	return createPagination(pageNumber, pageCount);
}
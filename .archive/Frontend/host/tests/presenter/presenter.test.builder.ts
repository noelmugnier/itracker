import { type IListBrandViewState, type IRemoveBrand, type IRetrieveBrands, ListBrandsViewModel, type IWriteViewModel, createPagination } from "@itracker/domain";
import { FakeNavigator } from "../test.fakes";

export class ListBrandsViewModelBuilder {
	private _retrieveBrands: IRetrieveBrands | undefined;
	private _removeBrand: IRemoveBrand | undefined;
	private _state: IListBrandViewState = {brands: [], error: undefined, isLoading: false, pagination: createPagination(1, 10)};

	constructor() {
	}

	withMockedBrands(retrieveBrands: IRetrieveBrands): ListBrandsViewModelBuilder {
		this._retrieveBrands = retrieveBrands;
		return this;
	}

	withRemoveBrand(removeBrand: IRemoveBrand): ListBrandsViewModelBuilder {
		this._removeBrand = removeBrand;
		return this;
	}

	withState(initialState: IListBrandViewState): ListBrandsViewModelBuilder {
		this._state = initialState;
		return this;
	}

	build(): ListBrandsViewModel {
		return new ListBrandsViewModel(this._retrieveBrands!, this._removeBrand!, new FakeNavigator(), this._state);
	}
}
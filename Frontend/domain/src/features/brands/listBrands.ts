import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { removeBrandCommand } from './removeBrand';
import { CustomError, PageSize, PageNumber } from '../../shared';
import { Brand, PaginatedBrands } from './models';
import { IRetrieveBrands } from './ports';

const initialState: ListBrandsState = { isLoading: false, error: undefined, brands: [], pageNumber: 1, pageSize: 10, totalBrands: 0 };

const listBrandsSlice = createSlice({
	name: 'brands',
	initialState: initialState,
	reducers: {
		brandsLoaded: (state, action: PayloadAction<ListBrandsResult>) => {
			state.brands = action.payload.brands;
			state.pageNumber = action.payload.pageNumber;
			state.pageSize = action.payload.pageSize;
			state.totalBrands = action.payload.totalBrands;
			
			state.error = initialState.error;
			state.isLoading = initialState.isLoading;
		}
	},
	extraReducers: (builder) => {
		builder.addCase(listBrandsQuery.pending, (state) => {
			state.isLoading = true;
		});
		builder.addCase(listBrandsQuery.fulfilled, (state, action) => {
			state.brands = action.payload.brands;
			state.pageNumber = action.payload.pageNumber;
			state.pageSize = action.payload.pageSize;
			state.totalBrands = action.payload.totalBrands;

			state.error = initialState.error;
			state.isLoading = initialState.isLoading;
		});
		builder.addCase(listBrandsQuery.rejected, (state, action) => {
			state.error = action.payload;
			state.isLoading = false;
		});
		builder.addCase(removeBrandCommand.fulfilled, (state, action) => {
			state.brands = state.brands.filter(b => b.id !== action.payload)
		});
	}
});

const createAppAsyncThunk = createAsyncThunk.withTypes<{
	rejectValue: CustomError,
	extra: { retrieveBrands: IRetrieveBrands }
}>()

export const listBrandsQuery = createAppAsyncThunk('brands/listBrands', async (query: ListBrandsQuery, { rejectWithValue, fulfillWithValue, extra }) => {
	const result = await extra.retrieveBrands.get(query.pageNumber, query.pageSize);
	if (result.err)
		return rejectWithValue(result.val);

	return fulfillWithValue<ListBrandsResult>(result.val);
});

export const { brandsLoaded } = listBrandsSlice.actions;

export type ListBrandsState = {
	isLoading: boolean;
	error: CustomError | undefined;
	brands: Brand[];
	pageNumber: PageNumber;
	pageSize: PageSize;
	totalBrands: number;
}

export type ListBrandsQuery = {
	pageNumber: PageNumber;
	pageSize: PageSize;
};

export type ListBrandsResult = PaginatedBrands;

export const listBrandsReducer = listBrandsSlice.reducer;
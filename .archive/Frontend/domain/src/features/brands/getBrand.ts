import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { CustomError } from '../../shared';
import { IRetrieveBrand } from './ports';
import { Brand, BrandId } from './models';

const initialState: BrandState = { isLoading: false, error: undefined, brand: undefined };

const brandSlice = createSlice({
	name: 'brand',
	initialState: initialState,
	reducers: {
		brandLoaded: (state, action: PayloadAction<Brand>) => {
			state.brand = action.payload;			
			state.error = initialState.error;
			state.isLoading = initialState.isLoading;
		}},
	extraReducers: (builder) => {
		builder.addCase(getBrandQuery.pending, (state) => {
			state.isLoading = true;
		});
		builder.addCase(getBrandQuery.fulfilled, (state, action) => {
			state.brand = action.payload
			state.error = initialState.error;
			state.isLoading = initialState.isLoading;
		});
		builder.addCase(getBrandQuery.rejected, (state, action) => {
			state.error = action.payload;
			state.isLoading = false;
		});
	}
});

const createAppAsyncThunk = createAsyncThunk.withTypes<{
	rejectValue: CustomError,
	extra: { retrieveBrand: IRetrieveBrand }
}>()

export const getBrandQuery = createAppAsyncThunk('brands/getBrand', async (query: GetBrandQuery, { rejectWithValue, fulfillWithValue, extra }) => {
	const result = await extra.retrieveBrand.get(query.brandId);
	if (result.err)
		return rejectWithValue(result.val);

	return fulfillWithValue<GetBrandResult>(result.val);
});

export type BrandState = {
	isLoading: boolean;
	error: CustomError | undefined;
	brand: Brand | undefined;
}

export type GetBrandQuery = {
	brandId: BrandId;
};

export type GetBrandResult = Brand;

export const { brandLoaded } = brandSlice.actions;
export const brandReducer = brandSlice.reducer;
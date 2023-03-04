import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { CustomError } from '../../shared';
import { ICreateBrand } from './ports';
import { CreateBrand } from './models';

const initialState: CreateBrandState = { createdId: undefined, isLoading: false, error: undefined };

const createBrandSlice = createSlice({
	name: 'createBrand',
	initialState: initialState,
	reducers: {},
	extraReducers: (builder) => {
		builder.addCase(createBrandCommand.pending, (state) => {
			state.createdId = undefined;
			state.isLoading = true;
		});
		builder.addCase(createBrandCommand.fulfilled, (state, action) => {
			state.createdId = action.payload;
			state.isLoading = initialState.isLoading;
			state.error = initialState.error;
		});
		builder.addCase(createBrandCommand.rejected, (state, action) => {
			state.createdId = undefined;
			state.isLoading = false;
			state.error = action.payload;
		});
	}
});

const createAppAsyncThunk = createAsyncThunk.withTypes<{
	rejectValue: CustomError,
	extra: { createBrand: ICreateBrand }
}>();

export const createBrandCommand = createAppAsyncThunk('brands/createBrand', async (command: CreateBrandCommand, { rejectWithValue, fulfillWithValue, extra }) => {
	const result = await extra.createBrand.create(command);
	if (result.err)
		return rejectWithValue(result.val);

	return fulfillWithValue<CreateBrandResult>(result.val);
});


export type CreateBrandState = {
	createdId: string | undefined;
	isLoading: boolean;
	error: CustomError | undefined;
}

export type CreateBrandCommand = CreateBrand;
export type CreateBrandResult = string;

export const createBrandReducer = createBrandSlice.reducer;
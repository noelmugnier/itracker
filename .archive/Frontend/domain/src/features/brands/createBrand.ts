import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { CustomError } from '../../shared';
import { ICreateBrand } from './ports';
import { CreateBrand } from './models';

const initialState: CreateBrandState = { createdId: undefined, isLoading: false, error: undefined };
const setState = (state: CreateBrandState, createdId : string | undefined, isLoading: boolean, error: CustomError | undefined) : CreateBrandState => {
	state.createdId = createdId;
	state.isLoading = isLoading;
	state.error = error;

	return state;
};

const createBrandSlice = createSlice({
	name: 'createBrand',
	initialState: initialState,
	reducers: {},
	extraReducers: (builder) => {
		builder.addCase(createBrandCommand.pending, (state) => {
			state = setState(state, undefined, true, undefined);
		});
		builder.addCase(createBrandCommand.fulfilled, (state, action) => {
			state = setState(state, action.payload, false, undefined);
		});
		builder.addCase(createBrandCommand.rejected, (state, action) => {
			state = setState(state, undefined, false, action.payload);
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
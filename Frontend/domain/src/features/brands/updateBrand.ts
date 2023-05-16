import { createAsyncThunk } from '@reduxjs/toolkit';
import { CustomError } from '../../shared';
import { Brand } from './models';
import { IUpdateBrand } from './ports';

const createAppAsyncThunk = createAsyncThunk.withTypes<{
	rejectValue: CustomError,
	extra: { updateBrand: IUpdateBrand }
}>();

export const updateBrandCommand = createAppAsyncThunk('brands/updateBrand', async (command: UpdateBrandCommand, { rejectWithValue, fulfillWithValue, extra }) => {
	const result = await extra.updateBrand.save(command);
	if (result.some)
		return rejectWithValue(result.val);

	return fulfillWithValue<Brand>(command);
});

export type UpdateBrandCommand = Brand;
export type UpdateBrandResult = Brand;

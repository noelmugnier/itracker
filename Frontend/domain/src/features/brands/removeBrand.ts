import { createAsyncThunk } from '@reduxjs/toolkit';
import { CustomError } from '../../shared';
import { IRemoveBrand } from './ports';
import { Brand } from './models';

const createAppAsyncThunk = createAsyncThunk.withTypes<{
	rejectValue: CustomError,
	extra: { removeBrand: IRemoveBrand }
}>()

export const removeBrandCommand = createAppAsyncThunk('brands/removeBrand', async (command: RemoveBrandCommand, { rejectWithValue, fulfillWithValue, extra }) => {
	const result = await extra.removeBrand.remove(command.id);
	if (result.some)
		return rejectWithValue(result.val);

	return fulfillWithValue<RemoveBrandResult>(command.id);
});

export type RemoveBrandCommand = Brand;
export type RemoveBrandResult = string;

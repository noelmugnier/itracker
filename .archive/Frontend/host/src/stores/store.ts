import { configureStore } from '@reduxjs/toolkit';
import svelteStoreEnhancer from "@utils/svelteStoreEnhancer";
import { useDispatch } from "@utils";
import { brandReducer, createBrandReducer, listBrandsReducer, listNotificationsReducer } from '@itracker/domain';
import { CreateBrandHttpAdapter, UpdateBrandHttpAdapter, RemoveBrandHttpAdapter, RetrieveBrandHttpAdapter, RetrieveBrandsHttpAdapter } from '@itracker/adapters';

export const apiBaseUrl = new URL(import.meta.env.VITE_API_URL);

export const store = configureStore({
	reducer: {
		brand: brandReducer,
		listBrands: listBrandsReducer,
		createBrand: createBrandReducer,
		listNotifications: listNotificationsReducer,
	},
	enhancers: [
		svelteStoreEnhancer
	],
	middleware: (getDefaultMiddleware) =>
		getDefaultMiddleware({
			thunk: {
				extraArgument: {
					retrieveBrands: new RetrieveBrandsHttpAdapter(fetch, apiBaseUrl),
					retrieveBrand: new RetrieveBrandHttpAdapter(fetch, apiBaseUrl),
					createBrand: new CreateBrandHttpAdapter(fetch, apiBaseUrl),
					removeBrand: new RemoveBrandHttpAdapter(fetch, apiBaseUrl),
					updateBrand: new UpdateBrandHttpAdapter(fetch, apiBaseUrl),
				}
			}
		})
});

export type RootState = ReturnType<typeof store.getState>

export type AppDispatch = typeof store.dispatch
export const useAppDispatch = (): AppDispatch => useDispatch<AppDispatch>()

export default store;
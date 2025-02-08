import { getContext } from 'svelte';
import type { Dispatch, Store } from '@reduxjs/toolkit';
import { STORE_KEY } from './constants';

export function useDispatch<T = unknown>(): Dispatch {
  const store: Store<T> = getContext(STORE_KEY);
  return store.dispatch;
}
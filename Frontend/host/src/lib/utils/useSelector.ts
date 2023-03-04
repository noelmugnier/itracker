import { getContext } from 'svelte';
import { derived, type Readable } from 'svelte/store';
import type { Selector } from '@reduxjs/toolkit';
import { STORE_KEY } from './constants';

export default function useSelector<T, S>(
  selector: Selector<T, S>,
  equalityFn?: (lhs: S, rhs: S) => boolean
): Readable<S> {
  if (!equalityFn) {
    equalityFn = (lhs: S, rhs: S) => lhs === rhs;
  }

  const store: Readable<T> = getContext(STORE_KEY);
  let lastSelectorValue: S;

  return derived(store, ($state: T, set) => {
    const selectorValue: S = selector($state);
    if (!equalityFn!(selectorValue, lastSelectorValue)) {
      lastSelectorValue = selectorValue;
      set(lastSelectorValue);
    }
  });
}
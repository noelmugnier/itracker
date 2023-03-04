import type { StoreEnhancerStoreCreator } from '@reduxjs/toolkit';

export default function svelteStoreEnhancer(
  createStoreApi: StoreEnhancerStoreCreator
): StoreEnhancerStoreCreator {
  return function (reducer, initialState) {
    const reduxStore = createStoreApi(reducer, initialState);
    return {
      ...reduxStore,
      subscribe(fn: (value: any) => void) {
        fn(reduxStore.getState());
        return reduxStore.subscribe(() => {
          fn(reduxStore.getState());
        });
      },
    };
  };
}
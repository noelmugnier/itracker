<script lang="ts">
	import { setContext } from 'svelte';
	import type { Store } from '@reduxjs/toolkit';
	import { STORE_KEY } from '@utils/constants';

	export let store: Store;
	setContext(STORE_KEY, store);
</script>

<slot />

<script lang="ts">
	import { useAppDispatch } from '@stores/store';
	import type { PageData } from './$types';
	import { brandLoaded } from '@itracker/domain';

	export let data: PageData;

	const dispatch = useAppDispatch();
	dispatch(brandLoaded(data));
</script>

<h1>{data.name}</h1>

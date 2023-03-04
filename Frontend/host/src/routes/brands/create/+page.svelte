<script lang="ts">
	import useSelector from '@lib/utils/useSelector';
	import { useAppDispatch, type RootState } from '@stores/store';
	import { createBrandCommand, type CreateBrand } from '@itracker/domain';

	const dispatch = useAppDispatch();

	let isLoading = useSelector((state: RootState) => state.createBrand.isLoading);
	let problem = useSelector((state: RootState) => state.createBrand.error);

	const vm: CreateBrand = { name: '' };
	$: showError = $problem != undefined;
</script>

<h1>Create brand</h1>
<a class="btn btn-default" href="/brands">Back</a>

{#if $isLoading}
	<p>Loading....</p>
{:else}
	<form>
		<input type="text" bind:value={vm.name} class="input input-bordered" />
		<button type="submit" on:click={() => dispatch(createBrandCommand(vm))} class="btn btn-primary">Create</button>
		
		{#if showError}
			<p class="from-error">{$problem?.status} {$problem?.message}</p>
		{/if}
	</form>
{/if}

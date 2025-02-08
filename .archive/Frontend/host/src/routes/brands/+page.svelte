<script lang="ts">
	import { useAppDispatch, type RootState } from '@stores/store';
	import type { PageData } from './$types';
	import useSelector from '@utils/useSelector';
	import { createPagination, removeBrandCommand, brandsLoaded, listBrandsQuery } from '@itracker/domain';

	export let data: PageData;

	const dispatch = useAppDispatch();
	dispatch(brandsLoaded(data));

	const vm = useSelector((state: RootState) => state.listBrands);
	$: pagination = createPagination($vm.pageNumber, $vm.pageSize);
</script>

<h1>Brands</h1>
<a class="btn btn-primary" href="/brands/create">Create</a>
<div class="overflow-x-auto">
	{#if !$vm.isLoading}
		<table class="table table-zebra w-full">
			<thead>
				<tr>
					<th>Name</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
				{#each $vm.brands as brand}
					<tr class="hover">
						<td>{brand.name}</td>
						<td>
							<a href="/brands/{brand.id}" class="btn btn-primary">View</a>
							<button class="btn btn-error" on:click={() => dispatch(removeBrandCommand(brand))}>Remove</button>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
	<button on:click={() => dispatch(listBrandsQuery(pagination.previousPage()))} class="btn btn-secondary">Previous</button>
	<button on:click={() => dispatch(listBrandsQuery(pagination.nextPage()))} class="btn btn-secondary">Next</button>
</div>

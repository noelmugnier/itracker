<script lang="ts">
	import { ListBrandsViewModel, Navigator, createPagination } from '@itracker/domain';
	import { RemoveBrandHttpAdapter, RetrieveBrandsHttpAdapter } from '@itracker/adapters';
	import { apiBaseUrl } from '@stores/store';
	import type { PageData } from './$types';
	import { invalidate } from '$app/navigation';

	export let data: PageData;

	const store = new ListBrandsViewModel(
		new RetrieveBrandsHttpAdapter(fetch, apiBaseUrl),
		new RemoveBrandHttpAdapter(fetch, apiBaseUrl),
		new Navigator(invalidate),
		{
			brands: data.brands,
			isLoading: false,
			pagination: createPagination(data.pageNumber, data.pageSize),
			error: undefined
		}
	);

	$: nextPage = `/?page=${$store.pagination.nextPage().pageNumber}&count=${$store.pagination.nextPage().pageSize}`;
	$: previousPage = `/?page=${$store.pagination.previousPage().pageNumber}&count=${$store.pagination.previousPage().pageSize}`;
</script>

<h1>Dashboard</h1>
<a class="btn btn-primary" href="/brands/create">Create</a>
<div class="overflow-x-auto">
	{#if !$store.isLoading}
		<table class="table table-zebra w-full">
			<thead>
				<tr>
					<th>Name</th>
					<th />
				</tr>
			</thead>
			<tbody>
				{#each $store.brands as brand}
					<tr class="hover">
						<td>{brand.name}</td>
						<td>
							<a href="/brands/{brand.id}" class="btn btn-primary">View</a>
							<button class="btn btn-error" on:click={() => store.remove(brand.id)}>Remove</button>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
	<a href="{previousPage}" class="btn btn-secondary" target="_self">Previous</a>
	<a href="{nextPage}" class="btn btn-secondary" target="_self">Next</a>
</div>

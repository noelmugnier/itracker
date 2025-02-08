import type { PageLoad } from './$types';
import { parseFromSearchParams } from '@itracker/domain';
import { apiBaseUrl } from '@stores/store';
import { RetrieveBrandsHttpAdapter } from '@itracker/adapters';
import { error } from '@sveltejs/kit';

export const load = (async ({fetch, url }) => {	
	let pagination = parseFromSearchParams(url, "page", "count");
	console.log(pagination);
	const result = await new RetrieveBrandsHttpAdapter(fetch, apiBaseUrl).get(pagination.pageNumber, pagination.pageSize);
	if (result.err)
		throw error(result.val.status, result.val.message);

	return { ...result.val };
}) satisfies PageLoad;
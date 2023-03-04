import type { PageLoad } from './$types';
import { error } from '@sveltejs/kit';
import { parseFromSearchParams } from '@itracker/domain';
import { apiBaseUrl } from '@stores/store';
import { RetrieveBrandsHttpAdapter } from '@itracker/adapters';

export const load = (async ({fetch, url }) => {	
	let pagination = parseFromSearchParams(url, "page", "count");
	const result = await new RetrieveBrandsHttpAdapter(fetch, apiBaseUrl).get(pagination.pageNumber, pagination.pageSize);
	if (result.err)
		throw error(result.val.status, result.val.message);

	return { ...result.val };
}) satisfies PageLoad;
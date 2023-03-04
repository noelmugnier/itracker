import type { PageLoad } from './$types';
import { error } from '@sveltejs/kit';
import { apiBaseUrl } from '@stores/store';
import { RetrieveBrandHttpAdapter } from '@itracker/adapters';

export const load = (async ({fetch, params }) => {	
	const result = await new RetrieveBrandHttpAdapter(fetch, apiBaseUrl).get(params.id);
	if (result.err)
		throw error(result.val.status, result.val.message);

	return { ...result.val };
}) satisfies PageLoad;
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vitest/config';

process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

export default defineConfig({
	plugins: [
		sveltekit()
	],
	test: {
		include: ['src/**/*.{test,spec}.{js,ts}']
	},	
});

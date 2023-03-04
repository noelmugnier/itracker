<script lang="ts">
	import StoreProvider from '@components/StoreProvider.svelte';
	import '../app.postcss';
	import store from '@stores/store';	
</script>

<StoreProvider {store}>
	<div class="navbar bg-base-100">
		<div class="navbar-start">
			<div class="dropdown">
				<label tabindex="0" class="btn btn-ghost btn-circle">
					<svg
						xmlns="http://www.w3.org/2000/svg"
						class="h-5 w-5"
						fill="none"
						viewBox="0 0 24 24"
						stroke="currentColor"
						><path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M4 6h16M4 12h16M4 18h7"
						/></svg
					>
				</label>
				<ul class="menu menu-compact dropdown-content mt-3 p-2 shadow bg-base-100 rounded-box w-52">
					<li><a href="/brands">Brands</a></li>
					<li><a href="/catalogs">Catalogs</a></li>
					<li><a href="/scrapers">Scrapers</a></li>
				</ul>
			</div>
		</div>
		<div class="navbar-center">
			<a class="btn btn-ghost normal-case text-xl" href="/">iTracker</a>
		</div>
	</div>
	<main>
		<slot />
	</main>
</StoreProvider>

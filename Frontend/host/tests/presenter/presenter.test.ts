import { describe, it, expect } from 'vitest';
import { ListBrandsViewModelBuilder } from './presenter.test.builder';

describe('Presenter', () => {
	it('should return a valid response', async () => {
		const vm = new ListBrandsViewModelBuilder().build();
		const response = await vm.next();
		expect(response).toEqual({ message: 'Hello World!' });
	});
});
import { type Brand, Pagination } from "@itracker/domain";
import type { IListBrandsVM } from "@itracker/application";
import type { Subscriber, Unsubscriber, Updater } from "svelte/store";
import { writable } from "svelte/store";

export class ListBrandsStore implements IListBrandsVM {
	private _update: (this:void, update: Updater<this>) => void;

	private _isLoading: boolean = false;
	private _pagination: Pagination = new Pagination();
	private _title: string = "";
	private _brands: Brand[] = [];
	private _errors: Error[] = [];

	public readonly subscribe: (this: void, run: Subscriber<this>, invalidate?: any) => Unsubscriber;

	constructor() {
		const { subscribe, update } = writable(this);
		this.subscribe = subscribe;
		this._update = update;
	}

	get IsLoading(): boolean {
		return this._isLoading;
	}

	set IsLoading(value: boolean) {
		this._update((store: this) => {
			store._isLoading = value;
			return store;
		});
	}

	get Pagination(): Pagination {
		return this._pagination;
	}

	set Pagination(value: Pagination) {
		this._update((store: this) => {
			store._pagination = value;
			return store;
		});
	}

	get Title(): string {
		return this._title;
	}

	set Title(value: string) {
		this._update((store: this) => {
			store._title = value;
			return store;
		});
	}

	get Brands(): Brand[] {
		return this._brands;
	}

	set Brands(value: Brand[]) {
		this._update((store: this) => {
			store._brands = value;
			return store;
		});
	}

	get Errors(): Error[] {
		return this._errors;
	}

	set Errors(value: Error[]) {
		this._update((store: this) => {
			store._errors = value;
			return store;
		});
	}
}

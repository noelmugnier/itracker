import { Pagination, createPagination } from "../../shared";
import { Brand } from "./models";
import { IRemoveBrand, IRetrieveBrands } from "./ports";

export interface IListBrandViewState {
	isLoading: boolean;
	error: string | undefined;
	brands: Brand[];
	pagination: Pagination;
}

export class ViewModel<T> implements IWriteViewModel<T>{
	private _subscribers: Subscriber<T>[] = [];
	private _state: T;

	constructor(state: T) {
		this._state = state;
	}

	subscribe: ViewModelListener<T> = (run: Subscriber<T>, invalidate?: Invalidator<T>): ViewModelUnsubscriber => {
		this._subscribers.push(run);
		run(this._state);

		return () => {
			this._subscribers = this._subscribers.filter(s => s !== run);
		};
	}

	set = (value: T): void => {
		this._state = { ...value };
		this._subscribers.forEach(subscriber => {
			subscriber(this._state);
		});
	}

	update = (fn: Updater<T>): void => {
		this.set(fn(this._state));
	}
}

export class ListBrandsViewModel extends ViewModel<IListBrandViewState> {
	constructor(
		private readonly _retrieveBrands: IRetrieveBrands,
		private readonly _removeBrand: IRemoveBrand,
		private readonly _navigator: INavigator,
		initialState?: IListBrandViewState | undefined) {
		super(initialState ?? {
			isLoading: false,
			error: undefined,
			brands: [],
			pagination: createPagination(1, 10)
		});
	}

	async next(): Promise<void> {
		const store = get(this);
		await this.list(store.pagination.nextPage());
	}

	async previous(): Promise<void> {
		const store = get(this);
		await this.list(store.pagination.previousPage());
	}

	async remove(id: string): Promise<void> {
		this.update(state => {
			state.isLoading = true;
			state.error = undefined;
			return state;
		});

		const data = get(this);
		const result = await this._removeBrand.remove(id);
		if (result.some)
			data.error = result.val.message;
		else
			data.brands = data.brands.filter(b => b.id !== id);

		data.isLoading = false;
		this.set(data);
		this._navigator.navigate("/brands");
	}

	private async list(pagination: Pagination): Promise<void> {
		this.update(state => {
			state.isLoading = true;
			state.error = undefined;
			return state;
		});

		const data = get(this);
		const result = await this._retrieveBrands.get(pagination.pageNumber, pagination.pageSize);
		if (result.err)
			data.error = result.val.message;
		else {
			data.brands = result.val.brands;
			data.pagination = pagination;
		}

		data.isLoading = false;
		this.set(data);
	}
}

export interface INavigator {
	navigate(path: string): Promise<void>;
}

export class Navigator implements INavigator {
	constructor(private _invalidate: (url: string | URL | ((url: URL) => boolean)) => Promise<void>) {
	}

	navigate(path: string): Promise<void> {
		return this._invalidate(path);
	}
}

export interface IViewModel<T> {
	subscribe: ViewModelListener<T>;
}

export interface IWriteViewModel<T> extends IViewModel<T> {
	set: (value: T) => void;
	update: (fn: Updater<T>) => void;
}

export function get<T>(store: IViewModel<T>): T {
	let value;
	store.subscribe((v: T) => (value = v))();
	return value as T;
}

export type ViewModelListener<T> = (run: Subscriber<T>, invalidate?: Invalidator<T>) => ViewModelUnsubscriber;
export type ViewModelUnsubscriber = () => void;
export type Invalidator<T> = (value?: T) => void;
export type Subscriber<T> = (value: T) => void;
export type Updater<T> = (value: T) => T;
export type ViewModelUpdater<T> = (fn: Updater<T>) => void;
export type ViewModelSetter<T> = (value: T) => void;

export type PageSize = number;

export const parsePageSize = (value: string | null | number): PageSize => {
	switch (typeof value) {
		case 'string':
			let parsedNumber = Number(value);
			return parsedNumber > 0 ? parsedNumber : 10;
		case 'number':
			return value > 0 ? value : 10;
		default:
			return 10;
	}
}
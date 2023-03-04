export type PageNumber = number;

export const parsePageNumber = (value: string | null | number): PageNumber => {
	switch (typeof value) {
		case 'string':
			let parsedNumber = Number(value);
			return parsedNumber > 0 ? parsedNumber : 1;
		case 'number':
			return value > 0 ? value : 1;
		default:
			return 1;
	}
}
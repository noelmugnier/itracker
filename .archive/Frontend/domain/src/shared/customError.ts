export type CustomError = { status: number, message: string };

export const createError = (message: string, status?: number | undefined): CustomError => { return { message : message, status : status ?? 400 } }

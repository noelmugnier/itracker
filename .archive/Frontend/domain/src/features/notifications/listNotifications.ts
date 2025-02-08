import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { NotificationStatus } from './enums';
import { Notification } from './models';
import { createBrandCommand } from '../brands';

const initialState: NotificationsState = { notifications: [] };

const listNotificationsSlice = createSlice({
	name: 'notifications',
	initialState: initialState,
	reducers: {
		notificationAdded: {
			reducer: (state, action: PayloadAction<Notification>) => {
				state.notifications.push(action.payload);
			},
			prepare: (status: NotificationStatus, title: string, message: string) => {
				return {
					payload: {
						title,
						message,
						status
					}
				}
			}
		}
	},
	extraReducers: (builder) => {
		builder.addCase(createBrandCommand.fulfilled, (state, action) => {
			state.notifications.push({
				title: "Brand created",
				message: `Brand has been created successfully with id ${action.payload}}`,
				status: NotificationStatus.Success
			});
		});
	}
});

export type NotificationsState = {
	notifications: Notification[];
}

export const {
	notificationAdded
} = listNotificationsSlice.actions;

export const listNotificationsReducer = listNotificationsSlice.reducer;
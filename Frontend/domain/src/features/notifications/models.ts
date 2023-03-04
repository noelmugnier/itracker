import { NotificationStatus } from "./enums";

export type Notification = {
	title: string;
	message: string;
	status: NotificationStatus;
}
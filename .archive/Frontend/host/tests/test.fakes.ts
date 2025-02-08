import type { INavigator, IWriteViewModel, Invalidator, Subscriber, Updater, ViewModelListener, ViewModelUnsubscriber } from "@itracker/domain";

export class FakeNavigator implements INavigator {
	currentUrl: string = '';

	constructor(currentUrl?: string | undefined) {
		this.currentUrl = currentUrl! ?? '/';
	}

	navigate(path: string): Promise<void> {
		this.currentUrl = path;
		return Promise.resolve();
	}
}


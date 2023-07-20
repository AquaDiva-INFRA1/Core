import {setApiConfig} from '@bexis2/bexis2-core-ui';


/** @type {import('./$types').PageLoad} */
export async function load() {

	if (import.meta.env.DEV) {
		setApiConfig('https://localhost:44345', 'davidschoene', '123456');
	}

	return {};
}
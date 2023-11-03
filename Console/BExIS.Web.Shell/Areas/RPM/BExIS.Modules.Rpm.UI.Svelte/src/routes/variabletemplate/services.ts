
// Implementations for all the calls for the pokemon endpoints.
//import Api from "./Api";
import type { VariableTemplateModel } from '$lib/components/datastructure/types';
import { Api } from '@bexis2/bexis2-core-ui';

/****************/
/* Overview Variable Template*/
/****************/


export const getVariableTemplates = async () => {
	try {
		const response = await Api.get('/rpm/VariableTemplate/GetVariableTemplates');
		return response.data;
	} catch (error) {
		console.error(error);
	}
};

export const update = async (data:VariableTemplateModel) => {
	try {
		const response = await Api.post('/rpm/VariableTemplate/Update', data);
		return response.data;
	} catch (error) {
		console.error(error);
	}
};

export const remove = async (id) => {
	try {
		const response = await Api.delete('/rpm/VariableTemplate/Delete',{id});
		return response.data;
	} catch (error) {
		console.error(error);
	}
};

export const getMeanings = async () => {
	try {
		const response = await Api.get('/rpm/variableTemplate/getMeanings');
		return response.data;
	} catch (error) {
		console.error(error);
	}
};
import { writable } from "svelte/store";
import { browser } from "$app/environment"

let defaultFilterVal = {
    itemCode: "",
    itemParentCode: "",
    statusValue: null,
    prodFromDate: null,
    prodToDate: null,
    disptFromDate: null,
    disptToDate: null,
    testsFromDate: null,
    testsToDate: null,
    compFromDate: null,
    compToDate: null,
    compSplFromDate: null,
    compSplToDate: null
};

if (browser) {
    let localStoredValue = localStorage.getItem("filterStore");
    if (localStoredValue !== null && localStoredValue !== undefined && localStoredValue !== "") {
        defaultFilterVal = JSON.parse(localStoredValue, (key, value) => {
            if (key.endsWith("Date") && value !== null)
                value = new Date(value);

                return value;
        });
    }
}

const filterStore = writable(defaultFilterVal);
export default filterStore;

filterStore.subscribe(value => {
    if (browser) return (localStorage.setItem("filterStore", JSON.stringify(value)))
});
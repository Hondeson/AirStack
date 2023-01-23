<script>
    import TextFilter from "./TextFilter.svelte";
    import StatusPickerFilter from "./StatusPickerFilter.svelte";
    import DateRangeFilter from "./DateRangeFilter.svelte";
    import { filterStore } from "../../stores/filterStore";
    import { subMonths } from "date-fns";
    import { createEventDispatcher } from "svelte";

    const dispatch = createEventDispatcher();

    const handleFormSubmit = () => {
        dispatch("submit");
    };

    let clearStatusSelection = false;

    let prodFromDate = subMonths(new Date(), 1);
    let prodToDate = new Date();
    $filterStore.prodFromDate = prodFromDate;
    $filterStore.prodToDate = prodToDate;

    const handleClearButton = () => {
        prodFromDate = subMonths(new Date(), 1);
        prodToDate = new Date();
        prodFromDate.setHours(0, 0, 0);
        prodToDate.setHours(23, 59, 59);

        $filterStore.prodFromDate = prodFromDate;
        $filterStore.prodToDate = prodToDate;

        $filterStore.itemCode = null;
        $filterStore.itemParentCode = null;
        $filterStore.disptFromDate = null;
        $filterStore.disptToDate = null;
        $filterStore.compFromDate = null;
        $filterStore.compToDate = null;
        $filterStore.compSplFromDate = null;
        $filterStore.compSplToDate = null;
        $filterStore.testsFromDate = null;
        $filterStore.testsToDate = null;

        $filterStore.statusValue = null;
        clearStatusSelection = true;

        dispatch("reset");    
    };
</script>

<div class="content">
    <form on:submit|preventDefault={handleFormSubmit}>
        <div style="display: flex;">
            <div class="mfiltr">
                <TextFilter
                    title="Kód airbagu:"
                    bind:filterValue={$filterStore.itemCode}
                />
                <TextFilter
                    title="Kód dílu:"
                    bind:filterValue={$filterStore.itemParentCode}
                />
                <div style="margin-top: 20px; width: 380px; margin-left: 10px;">
                    <StatusPickerFilter
                        bind:flagValue={$filterStore.statusValue}
                        clearValue={clearStatusSelection}
                    />
                </div>
            </div>

            <div style="display: flex; flex-wrap: wrap; margin-left: 30px;">
                <div>
                    <DateRangeFilter
                        title="Vstup do výroby"
                        bind:fromDate={$filterStore.prodFromDate}
                        bind:toDate={$filterStore.prodToDate}
                    />
                    <DateRangeFilter
                        title="Expedice"
                        bind:fromDate={$filterStore.disptFromDate}
                        bind:toDate={$filterStore.disptToDate}
                    />
                </div>

                <div>
                    <DateRangeFilter
                        title="Reklamace zákazníka"
                        bind:toDate={$filterStore.compFromDate}
                        bind:fromDate={$filterStore.compToDate}
                    />
                    <DateRangeFilter
                        title="Reklamace dodavateli"
                        bind:fromDate={$filterStore.compSplFromDate}
                        bind:toDate={$filterStore.compSplToDate}
                    />
                </div>

                <div>
                    <DateRangeFilter
                        title="Testy"
                        bind:fromDate={$filterStore.testsFromDate}
                        bind:toDate={$filterStore.testsToDate}
                    />
                </div>
            </div>
        </div>

        <div class="form-buttons">
            <button type="submit" class="full-button" style="margin-right: 15px;">HLEDEJ</button>
            <button type="button" class="full-button" on:click={handleClearButton}>SMAZAT FILTR </button>
        </div>
    </form>
</div>

<style>
    .content {
        padding: 10px;
        border: 1px black solid;
        border-color: black;
    }

    .mfiltr {
        margin: 32px 0px;
        display: flex;
        flex-direction: column;
    }

    .form-buttons {
        padding: 30px 0px 0px 0px;
    }
</style>

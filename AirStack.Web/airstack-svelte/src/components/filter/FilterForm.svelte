<script>
    import TextFilter from "./TextFilter.svelte";
    import StatusFilter from "./StatusFilter.svelte";
    import DateRangeFilter from "./DateRangeFilter.svelte";
    import filterStore from "../../stores/filterStore";
    import { subMonths } from "date-fns";
    import { createEventDispatcher, onMount } from "svelte";
    

    const dispatch = createEventDispatcher();

    const handleFormSubmit = () => {
        dispatch("submit");
    };

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
                <StatusFilter bind:flagValue={$filterStore.statusValue} />
            </div>

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
        <button type="submit"> aplikovat filtr </button>
        <button on:click={handleClearButton}> smazat filtr </button>
    </form>
</div>

<style>
    .content {
        padding: 10px;
        background-color: lightgray;
        box-shadow: 0 4px 6px 0 rgba(0, 0, 0, 0.4);
        border-radius: 5px;
    }

    button {
        background-color: rgb(120, 82, 178);
        color: white;
        font-size: 16px;
        border-width: 1px;
        border-style: solid;
        border-color: white;
        padding: 8px 15px;
        border-radius: 16px;
        cursor: pointer;
        transition: background-color 0.2s;
        height: 40px;
        width: 130px;
        margin: 30px 0px;
    }

    button:hover {
        background-color: rgb(104, 55, 176);
    }
</style>

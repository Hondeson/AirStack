<script>
    import getPath from "../apiRequestHelper";
    import { onMount } from "svelte";
    import { appliedFilterStore, filterStore } from "../stores/filterStore";
    import { writable } from "svelte/store";
    import { createTable, Subscribe, Render } from "svelte-headless-table";
    import FilterForm from "../components/filter/FilterForm.svelte";
    import ExportButton from "../components/ExportButton.svelte";
    import { format } from "date-fns";
    import PaginationButtons from "../components/PaginationButtons.svelte";
    import {
        addPagination,
        addColumnFilters,
    } from "svelte-headless-table/plugins";

    let gridData = writable([]);

    const table = createTable(gridData, {
        page: addPagination({ initialPageSize: 50 }),
        colFilter: addColumnFilters(),
    });

    const handleApplyFilter = () => {
        appliedFilterStore.set(structuredClone($filterStore));
        loadGridData(0, 150);
        pageIndex.set(0);
        actualPage = 1;
    };

    const columns = table.createColumns([
        table.column({
            header: "ID",
            accessor: "id",
        }),
        table.column({
            header: "Kód airbagu",
            accessor: "code",
        }),
        table.column({
            header: "Kód dílu",
            accessor: "parentCode",
        }),
        table.column({
            header: "Aktuální status",
            accessor: "actualStatus",
        }),
        table.column({
            header: "Datum vstupu do výroby",
            accessor: "productionDate",
        }),
        table.column({
            header: "Datum expedice",
            accessor: "dispatchedDate",
        }),
        table.column({
            header: "Datum testů",
            accessor: "testsDate",
        }),
        table.column({
            header: "Datum reklamace zákazníka",
            accessor: "complaintDate",
        }),
        table.column({
            header: "Datum reklamace dodavateli",
            accessor: "complaintToSupplierDate",
        }),
    ]);

    const { headerRows, pageRows, tableAttrs, tableBodyAttrs, pluginStates } =
        table.createViewModel(columns);

    const { pageIndex, pageCount, pageSize, hasPreviousPage, hasNextPage } =
        pluginStates.page;

    let isBusy = false;
    let totalItemCount = 0;
    let actualPage = 1;
    $: totalPages = Math.ceil(totalItemCount / $pageSize);

    const getFilterParamsObj = () => {
        return {
            statusEnum: $appliedFilterStore.statusValue,
            codeLike: $appliedFilterStore.itemCode,
            parentCodeLike: $appliedFilterStore.itemParentCode,
            productionFrom: $appliedFilterStore.prodFromDate,
            productionTo: $appliedFilterStore.prodToDate,
            dispatchedFrom: $appliedFilterStore.disptFromDate,
            dispatchedTo: $appliedFilterStore.disptToDate,
            testsFrom: $appliedFilterStore.testsFromDate,
            testsTo: $appliedFilterStore.testsToDate,
            complaintFrom: $appliedFilterStore.compFromDate,
            complaintTo: $appliedFilterStore.compToDate,
            complaintSuplFrom: $appliedFilterStore.compSplFromDate,
            complaintSuplTo: $appliedFilterStore.compSplToDate,
        };
    };

    let isError = false;
    const loadGridData = async (offsetNum, fetchNum) => {
        isBusy = true;
        isError = false;

        try {
            const req = await fetch(
                getPath("api/Item", {
                    ...getFilterParamsObj(),
                    offset: offsetNum,
                    fetch: fetchNum,
                })
            );

            var json = await req.json();
        } catch (e) {
            isBusy = false;
            isError = true;
            console.log(e);
            return;
        }

        const formattedData = json.map((obj) => {
            const newObj = {};
            for (const [key, value] of Object.entries(obj)) {
                if (key.endsWith("Date") && value != null) {
                    const date = new Date(value);
                    newObj[key] = format(date, "yyyy-MM-dd HH:mm:ss");
                    continue;
                }

                if (key === "actualStatus") {
                    newObj[key] = getActualStatusFormatted(value);
                    continue;
                }

                newObj[key] = value || "";
            }
            return newObj;
        });

        gridData.set(formattedData);

        totalItemCount = await getTotalItemCount();
        isBusy = false;
    };

    const handleExportFileGetUrl = () => {
        console.log(getFilterParamsObj());
        return getPath("api/Item/GetFile", getFilterParamsObj());
    };

    const getTotalItemCount = async () => {
        const req = await fetch(
            getPath("api/Item/GetItemCount", getFilterParamsObj())
        );

        let json = await req.json();
        let count = json;

        return count;
    };

    const getActualStatusFormatted = (name) => {
        if (name === "Production") return "Produkce";
        else if (name === "Tests") return "Testy";
        else if (name === "Dispatched") return "Expedováno";
        else if (name === "Complaint") return "Reklamace zákazníka";
        else if (name === "ComplaintToSupplier") return "Reklamace dodavateli";

        return "Chyba webu!";
    };

    //#region pagination
    const handleNextPage = () => {
        pageIndex.update((actual) => {
            if (actualPage === totalPages) return actual;

            actualPage = actualPage + 1;
            if ($hasNextPage) return actual + 1;

            loadGridData(actualPage * $pageSize, 50);
            return actual;
        });
    };

    const handlePreviousPage = () => {
        pageIndex.update((actual) => {
            if (actualPage == 1) return actual;

            actualPage = actualPage - 1;
            if ($hasPreviousPage) return actual - 1;

            let num = actualPage * $pageSize;
            loadGridData(actualPage == 1 ? 0 : num, 50);
            return actual;
        });
    };
    //#endregion

    const handleResetFilter = () => {
        appliedFilterStore.set(structuredClone($filterStore));
    };

    onMount(() => {
        loadGridData(0, 150);
    });
</script>

<svelte:head>
    <title>Air Stack</title>
</svelte:head>

<div class="header">
    <h1>AIR STACK</h1>
</div>

<div class="content">
    <div class="filter-content">
        <FilterForm
            on:submit={handleApplyFilter}
            on:reset={handleResetFilter}
        />
    </div>

    <div class="grid-top">
        <ExportButton getUrl={handleExportFileGetUrl} title="Export .csv" />

        {#if isBusy}
            <p style="margin: 0px;">načítám...</p>
        {:else if isError}
            <p style="margin: 0px; color: red;">Error</p>
        {/if}

        <PaginationButtons
            {actualPage}
            {totalPages}
            on:nextPageClicked={handleNextPage}
            on:previousPageClicked={handlePreviousPage}
        />
    </div>

    <table {...$tableAttrs}>
        <thead>
            {#each $headerRows as headerRow (headerRow.id)}
                <Subscribe rowAttrs={headerRow.attrs()} let:attrs>
                    <tr {...attrs}>
                        {#each headerRow.cells as cell (cell.id)}
                            <Subscribe
                                attrs={cell.attrs()}
                                let:attrs
                                props={cell.props()}
                                let:props
                            >
                                <th {...attrs} class="unselectable">
                                    <div>
                                        <Render of={cell.render()} />
                                    </div>
                                    {#if props.colFilter !== undefined}
                                        <Render of={props.colFilter.render} />
                                    {/if}
                                </th>
                            </Subscribe>
                        {/each}
                    </tr>
                </Subscribe>
            {/each}
        </thead>
        <tbody {...$tableBodyAttrs}>
            {#each $pageRows as row (row.id)}
                <Subscribe rowAttrs={row.attrs()} let:rowAttrs>
                    <tr {...rowAttrs}>
                        {#each row.cells as cell (cell.id)}
                            <Subscribe attrs={cell.attrs()} let:attrs>
                                <td {...attrs}>
                                    <Render of={cell.render()} />
                                </td>
                            </Subscribe>
                        {/each}
                    </tr>
                </Subscribe>
            {/each}
        </tbody>
    </table>

    <div class="grid-bottom">
        <PaginationButtons
            {actualPage}
            {totalPages}
            on:nextPageClicked={handleNextPage}
            on:previousPageClicked={handlePreviousPage}
        />
    </div>
</div>

<style>
    table {
        border-spacing: 0;
        border-top: 1px solid black;
        border-left: 1px solid black;
    }

    th,
    td {
        border-bottom: 1px solid black;
        border-right: 1px solid black;
        padding: 0.5rem;
    }

    h1 {
        color: black;
        font-size: 26px;
        margin: 0px 16px;
    }
    .header {
        height: 50px;
        display: flex;
        position: fixed;
        background: white;
        top: 0;
        left: 0;
        right: 0;
        justify-content: center;
        align-items: center;
        border-bottom: 1px black solid;
        z-index: 100;
    }

    .grid-top {
        display: flex;
        justify-content: space-between;
        margin: 30px 0px 10px 0px;
    }

    .grid-bottom {
        display: flex;
        justify-content: end;
        margin: 15px 0px 10px 0px;
    }

    .content {
        margin: 80px 30px 30px 30px;
        display: flex;
        flex-direction: column;
        justify-content: center;
    }

    h1,
    .unselectable {
        -webkit-touch-callout: none;
        -webkit-user-select: none;
        -khtml-user-select: none;
        -moz-user-select: none;
        -ms-user-select: none;
        user-select: none;
    }
</style>

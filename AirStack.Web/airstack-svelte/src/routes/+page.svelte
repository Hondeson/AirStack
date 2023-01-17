<script>
    import getPath from "../apiRequestHelper";
    import { onMount } from "svelte";
    import { subMonths } from "date-fns";
    import { readable, writable } from "svelte/store";
    import {
        createTable,
        Subscribe,
        Render,
        createRender,
    } from "svelte-headless-table";
    import DateRangeFilter from "../components/DateRangeFilter.svelte";
    import ExportButton from "../components/ExportButton.svelte";
    import formatDate from "date-fns/format";
    import PaginationButtons from "../components/PaginationButtons.svelte";
    import TextFilter from "../components/TextFilter.svelte";
    import StatusFilter from "../components/StatusFilter.svelte";
    import {
        addSortBy,
        addPagination,
        addResizedColumns,
        addColumnFilters,
        textPrefixFilter,
    } from "svelte-headless-table/plugins";

    let gridData = writable([]);

    const table = createTable(gridData, {
        page: addPagination({ initialPageSize: 50 }),
        colFilter: addColumnFilters(),
    });

    //#region gridFilterHandlers
    let prodFromDate;
    let prodToDate;

    const handleApplyFilter = () => {
        loadGridData(0, 150);
        pageIndex.set(0);
        actualPage = 0;
    };
    //#endregion

    //musí být definováno
    const handleColFilter = (filterValue, value) => {
        return true;
    };

    const columns = table.createColumns([
        table.column({
            header: "ID",
            accessor: "id",
        }),
        table.column({
            header: "Kód airbagu",
            accessor: "code",
            plugins: {
                colFilter: {
                    fn: handleColFilter,
                    render: ({ filterValue }) =>
                        createRender(TextFilter, {
                            filterValue: filterValue,
                        }),
                },
            },
        }),
        table.column({
            header: "Kód dílu",
            accessor: "parentCode",
            plugins: {
                colFilter: {
                    fn: handleColFilter,
                    render: ({ filterValue }) =>
                        createRender(TextFilter, {
                            filterValue: filterValue,
                        }),
                },
            },
        }),
        table.column({
            header: "Aktuální status",
            accessor: "actualStatus",
            plugins: {
                colFilter: {
                    fn: handleColFilter,
                    render: ({ filterValue }) =>
                        createRender(StatusFilter, {
                            filterValue: filterValue,
                        }),
                },
            },
        }),
        table.column({
            header: "Datum vstupu do výroby",
            accessor: "productionDate",
            plugins: {
                colFilter: {
                    fn: handleColFilter,
                    initialFilterValue: {
                        fromDate: subMonths(new Date(), 1),
                        toDate: new Date(),
                        isoStringFromDate: null,
                        isoStringToDate: null,
                    },
                    render: ({ filterValue }) =>
                        createRender(DateRangeFilter, { filterValue }),
                },
            },
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
    let actualPage = 0;
    $: totalPages = Math.ceil(totalItemCount / $pageSize);
    //TODO: chyba připejení s API
    const loadGridData = async (offsetNum, fetchNum) => {
        isBusy = true;
        const req = await fetch(
            getPath("api/Item", {
                offset: offsetNum,
                fetch: fetchNum,
                productionFrom: prodFromDate,
                productionTo: prodToDate,
            })
        );

        let json = await req.json();

        const formattedData = json.map((obj) => {
            const newObj = {};
            for (const [key, value] of Object.entries(obj)) {
                if (key.endsWith("Date") && value != null) {
                    const date = new Date(value);
                    newObj[key] = formatDate(date, "yyyy-MM-dd HH:mm:ss");
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

    const getTotalItemCount = async () => {
        const req = await fetch(
            getPath("api/Item/GetItemCount", {
                statusEnum: 1,
                productionFrom: prodFromDate,
                productionTo: prodToDate,
            })
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
            if (actualPage == 0) return actual;

            actualPage = actualPage - 1;
            if ($hasPreviousPage) return actual - 1;

            let num = actualPage * $pageSize;
            loadGridData(num < 0 ? 0 : num, 50);
            return actual;
        });
    };
    //#endregion

    onMount(() => {
        loadGridData(0, 150);
    });
</script>

<svelte:head>
    <title>Air Stack</title>
</svelte:head>

<div class="header">
    <h1>Air Stack</h1>
</div>

<div class="content">
    <div class="grid-top">
        <!-- TODO: bind parametr na filtr -->
        <ExportButton
            url="{getPath('api/Item/GetFile', {
                statusEnum: 0,
                productionFrom: prodFromDate,
                productionTo: prodToDate,
            })}}"
        />

        <button on:click={handleApplyFilter}> aplikovat filtr </button>

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

    <PaginationButtons
        {actualPage}
        {totalPages}
        on:nextPageClicked={handleNextPage}
        on:previousPageClicked={handlePreviousPage}
    />
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

    .header {
        height: 60px;
        display: flex;
        flex-direction: row;
        justify-content: center;
        background-color: rgb(120, 82, 178);
        align-items: center;
    }

    .grid-top {
        display: flex;
        justify-content: space-between;
        margin: 15px 0px 0px 0px;
    }

    .content {
        margin: 30px;
        display: flex;
        flex-direction: column;
        justify-content: center;
    }

    h1 {
        color: white;
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

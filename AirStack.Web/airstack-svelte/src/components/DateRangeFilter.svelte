<script>
    import { cs } from "date-fns/locale";
    import { DateInput, localeFromDateFnsLocale } from "date-picker-svelte";

    export let filterValue;
    const { fromDate, toDate } =
        $filterValue;

    $: setUtcTimes(), fromDate, toDate;

    const setUtcTimes = () => {
        $filterValue.isoStringFromDate = fromDate.toISOString();
        $filterValue.isoStringToDate = toDate.toISOString();
    };

    fromDate.setHours(0, 0, 0);
    toDate.setHours(23, 59, 59);

    let locale = localeFromDateFnsLocale(cs);
</script>

<div class="main">
    <div class="filter">
        <p>od:</p>
        <DateInput
            bind:value={$filterValue.fromDate}
            {locale}
            closeOnSelection={true}
            placeholder="yyyy-MM-dd hh:mm:ss"
        />
    </div>
    <div class="filter">
        <p>do:</p>
        <DateInput
            bind:value={$filterValue.toDate}
            {locale}
            closeOnSelection={true}
            placeholder="yyyy-MM-dd hh:mm:ss"
        />
    </div>
</div>

<style>
    .filter {
        display: flex;
        align-items: center;
        margin: 10px;
    }

    p {
        font-size: 16px;
        margin: 0px 10px;
    }

    :root {
        --date-input-width: auto;
    }
</style>

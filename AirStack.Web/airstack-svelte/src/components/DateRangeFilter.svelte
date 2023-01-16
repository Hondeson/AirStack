<script>
    import { subMonths } from "date-fns";
    import { cs } from "date-fns/locale";
    import { DateInput, localeFromDateFnsLocale } from "date-picker-svelte";

    export let fromDate = subMonths(new Date(), 1);
    export let toDate = new Date();

    export let isoFromDate;
    export let isoToDate;

    $: setUtcTimes(), fromDate, toDate;

    fromDate.setHours(0, 0, 0);
    toDate.setHours(23, 59, 59);

    const setUtcTimes = () => {
        isoFromDate = fromDate.toISOString();
        isoToDate = toDate.toISOString();
    };

    let locale = localeFromDateFnsLocale(cs);
</script>

<div class="main">
    <div class="filter">
        <p>od:</p>
        <DateInput
            bind:value={fromDate}
            {locale}
            closeOnSelection={true}
            placeholder="yyyy-MM-dd hh:mm:ss"
        />
    </div>
    <div class="filter">
        <p>do:</p>
        <DateInput
            bind:value={toDate}
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

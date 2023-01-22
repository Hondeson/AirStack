<script>
    import { cs } from "date-fns/locale";
    import { DateInput, localeFromDateFnsLocale } from "date-picker-svelte";

    export let title;
    export let fromDate;
    export let toDate;

    let locale = localeFromDateFnsLocale(cs);
</script>

<div>
    <p>{title}</p>
</div>

<div class="content">
    <div class="filter">
        <p>od:</p>
        <DateInput
            bind:value={fromDate}
            {locale}
            closeOnSelection={true}
            placeholder=""
        />
    </div>
    <div class="filter">
        <p>do:</p>
        <DateInput
            bind:value={toDate}
            {locale}
            closeOnSelection={true}
            placeholder=""
        />
    </div>
</div>

<style>
    .content {
        margin: 10px;
    }

    .filter {
        display: flex;
        align-items: center;
        margin-bottom: 5px;
    }

    p {
        margin: 5px 10px;
    }

    :root {
        --date-input-width: auto;
    }
</style>

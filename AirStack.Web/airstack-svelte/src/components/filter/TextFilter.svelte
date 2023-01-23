<script>
    export let filterValue;
    export let title = "";
    export let placeholder = "Kód";
</script>

<div class="content">
    <p>{title}</p>

    <input
        type="text"
        bind:value={filterValue}
        on:click|stopPropagation
        {placeholder}
    />
</div>

<style>
    p {
        margin: 8px;
    }

    .content {
        margin: 5px;
        align-items: center;
        justify-content: space-between;
        display: flex;
    }

    input{
        border: 1px black solid;
        padding: 5px;
        border-radius: 3px;
    }
</style>

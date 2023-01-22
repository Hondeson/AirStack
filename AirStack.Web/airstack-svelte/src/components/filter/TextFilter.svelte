<script>
    export let filterValue;
    export let title = "";
    export let placeholder = "Hledej";
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
p{
    margin: 8px;
}

    .content {
        margin: 5px;
        align-items: center;
        display: flex;
    }
</style>

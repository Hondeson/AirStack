<script>
    import { createEventDispatcher } from "svelte";
    const dispatch = createEventDispatcher();

    export let actualPage;
    export let totalPages;

    const nextClicked = () => {
        dispatch("nextPageClicked");
    };

    const previousPageClicked = () => {
        dispatch("previousPageClicked");
    };
</script>

<div class="content">
    <button class="empty-button" on:click={previousPageClicked}>zpět</button>

    <div class="text-content">
        {#if totalPages === 0}
            <p>0</p>
        {:else}
            <p>{actualPage}</p>
        {/if}
        <p>z</p>
        <p>{totalPages}</p>
    </div>
    <button class="empty-button" on:click={nextClicked}>další</button>
</div>

<style>
    .content {
        align-items: center;
        display: flex;
        flex-direction: row;
        justify-content: end;
    }

    p {
        margin: 0px 3px;
        font-family: "Times New Roman", Times, serif;
    }

    .text-content {
        display: flex;
        margin: 0px 10px;
    }

    .empty-button {
        width: 130px;
    }
</style>

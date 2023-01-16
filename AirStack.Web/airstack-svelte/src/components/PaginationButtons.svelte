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
    <button on:click={previousPageClicked}>zpět</button>
    <p>{actualPage} z {totalPages}</p>
    <button on:click={nextClicked}>další</button>
</div>

<style>
    .content {
        align-items: center;
        display: flex;
        flex-direction: row;
        justify-content: end;
        margin: 10px 0px;
    }

    button {
        background-color: rgb(120, 82, 178);
        color: white;
        font-size: 16px;
        border-width: 1px;
        border-style: solid;
        border-color: white;
        padding: 8px 15px;
        cursor: pointer;
        transition: background-color 0.2s;
        border-radius: 16px;
    }

    button:hover {
        background-color: rgb(104, 55, 176);
    }

    p{
        margin: 0px 5px;
    }
</style>

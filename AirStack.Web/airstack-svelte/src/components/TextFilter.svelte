<script>
    export let filterValue; //: Writable<string>;
    export let placeholder = "Hledej"; //: Readable<string[]>;
</script>

<input
    type="text"
    bind:value={$filterValue}
    on:click|stopPropagation
    {placeholder}
/>

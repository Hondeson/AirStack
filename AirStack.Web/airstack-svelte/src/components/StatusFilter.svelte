<script>
    import Svelecte from "svelecte";
    export let filterValue;

    let dataset = [
        { name: "Production", value: "Produkce" },
        { name: "Tests", value: "Testy" },
        { name: "Dispatched", value: "Expedováno" },
        { name: "Complaint", value: "Reklamace zákazníka" },
        { name: "ComplaintToSupplier", value: "Reklamace dodavateli" },
    ];

    let options = dataset.map((opt) => opt.value);

    let selection = [];
    let value = null;
</script>

<Svelecte
    {options}
    bind:readSelection={selection}
    bind:value
    multiple={true}
    placeholder="Stav"
    style="width: 270px;"
/>

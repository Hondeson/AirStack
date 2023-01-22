<script>
    import Svelecte from "svelecte";

    export let flagValue = null;
    export let clearValue = false;

    let selectedValueIndexes = null;
    const dataset = [
        { index: 0, name: "Production", displayName: "Produkce", flag: 1 },
        { index: 1, name: "Tests", displayName: "Testy", flag: 2 },
        { index: 2, name: "Dispatched", displayName: "Expedováno", flag: 4 },
        {
            index: 3,
            name: "Complaint",
            displayName: "Reklamace zákazníka",
            flag: 8,
        },
        {
            index: 4,
            name: "ComplaintToSupplier",
            displayName: "Reklamace dodavateli",
            flag: 16,
        },
    ];
    let options = dataset.map((opt) => opt.displayName);

    //potřeba, aby se necyklilo při inicializace
    let isInit = true;
    const restoreValue = () => {
        isInit = true;
        if (flagValue !== null && flagValue !== 0) {
            let reverseDataSet = dataset.reverse();
            var f = flagValue;
            selectedValueIndexes = [];
            reverseDataSet.forEach((obj) => {
                if (f - obj.flag >= 0) {
                    selectedValueIndexes = [...selectedValueIndexes, obj.index];
                    f = f - obj.flag;
                }
            });

            dataset.reverse();
        }

        isInit = false;
    };

    restoreValue();

    //propisuje změnu do export flagValue proměnné
    $: setFlagVal(), selectedValueIndexes;
    const setFlagVal = () => {
        if (isInit) return;

        if (
            selectedValueIndexes === null ||
            selectedValueIndexes.length === 0
        ) {
            flagValue = null;
            return;
        }

        let val = 0;
        selectedValueIndexes.forEach((i) => {
            let obj = dataset.at(i);
            val = val + obj.flag;
        });

        if (val > 0) flagValue = val;
    };

    //propisuje změny ve flagValue na selection
    $: clearValue, resetSelection();
    const resetSelection = () => {
        if (!clearValue) return;
        if (flagValue !== null) return;

        selectedValueIndexes = null;
        clearValue = false;
    };
</script>

<Svelecte
    {options}
    bind:value={selectedValueIndexes}
    multiple={true}
    placeholder="Aktuální stav"
/>

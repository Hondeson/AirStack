<script>
    import Svelecte from "svelecte";
    export let flagValue = null;

    const dataset = [
        { index: 1, name: "Production", value: "Produkce", flag: 1 },
        { index: 2, name: "Tests", value: "Testy", flag: 2 },
        { index: 3, name: "Dispatched", value: "Expedováno", flag: 4 },
        { index: 4, name: "Complaint", value: "Reklamace zákazníka", flag: 8 },
        {
            index: 5,
            name: "ComplaintToSupplier",
            value: "Reklamace dodavateli",
            flag: 16,
        },
    ];

    let options = dataset.map((opt) => opt.value);

    let value = null;

    //potřeba, aby se necyklilo při inicializace
    let isInit = true;
    const restoreValue = () => {
        if (flagValue !== null && flagValue !== 0) {
            let reverseDataSet = dataset.reverse();
            var f = flagValue;
            value = [];
            reverseDataSet.forEach((obj) => {
                if (f - obj.flag >= 0) {
                    value = [...value, obj.index];
                    f = f - obj.flag;
                }
            });
        }

        isInit = false;
    };

    restoreValue();

    //propisuje změnu do export flagValue proměnné
    $: setFlagVal(), value;
    const setFlagVal = () => {
        if (isInit) return;
        if (value === null || value.length === 0) {
            flagValue = null;
            return;
        }

        let val = 0;
        value.forEach((index) => {
            val = val + dataset[index].flag;
        });

        if (val > 0) flagValue = val;
    };

    //propisuje změny ve flagValue na selection
    $: flagValue, resetSelection();
    const resetSelection = () => {
        if (isInit) return;

        if (flagValue !== null) return;

        value = null;
    };
</script>

<Svelecte {options} bind:value multiple={true} placeholder="Aktuální stav" />

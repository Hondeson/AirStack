<script>
    export let getUrl;

    let url;
    const updateUrl = () => {
        const a = document.createElement("a");
        a.style.display = "none";
        a.href = getUrl();
        document.body.appendChild(a);
        a.click();
        setTimeout(() => {
            document.body.removeChild(a);
        }, 100);
    };
</script>

<form on:submit|preventDefault={updateUrl}>
    <button type="submit" value="Export">Export</button>
</form>

<style>
    button {
        background-color: rgb(50, 163, 48);
        color: white;
        font-size: 16px;
        border-width: 1px;
        border-style: solid;
        border-color: white;
        padding: 8px 15px;
        border-radius: 16px;
        cursor: pointer;
        transition: background-color 0.2s;
        height: 40px;
        width: 130px;
    }

    button:hover {
        background-color: rgb(37, 122, 35);
    }
</style>

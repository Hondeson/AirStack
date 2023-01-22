<script>
    export let getUrl;
    export let title = "Export";

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
    <button type="submit" value="Export" class="empty-button">{title}</button>
</form>

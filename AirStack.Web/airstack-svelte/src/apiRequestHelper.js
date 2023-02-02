import { PUBLIC_API_PATH } from '$env/static/public';

const getPath = (path, paramObject) => {
    path = PUBLIC_API_PATH + "/" + path;

    let url = new URL(path);
    url = addSearchParams(url, paramObject);

    return url.href;
}

const addSearchParams = (url, params = {}) => {

    //odstraní undefined, null a whitespacestring
    Object.keys(params).forEach(key => {
        if (params[key] === undefined || params[key] === null || /^\s*$/.test(params[key])) {
            delete params[key];
        }

        if (params[key] instanceof Date)
            params[key] = new Date(params[key]).toISOString();
    });

    return new URL(
        `${url.origin}${url.pathname}?${new URLSearchParams([
            ...Array.from(url.searchParams.entries()),
            ...Object.entries(params),
        ])}`
    );
}

const removeEmpty = (obj) => {
    let newObj = {};
    Object.keys(obj).forEach((key) => {
        if (obj[key] === Object(obj[key])) newObj[key] = removeEmpty(obj[key]);
        else if (obj[key] !== undefined) newObj[key] = obj[key];
    });
    return newObj;
};

export default getPath;
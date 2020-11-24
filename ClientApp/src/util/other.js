

//クエリストリングの条件をオブジェクトとして取得する。
export const getObjFromQueryString = () => {
    //クエリストリングで検索条件を指定された場合。
    const urlParams = new URLSearchParams(location.search);
    let obj = {}
    for (const key of urlParams.keys()) {
        obj[key] = urlParams.get(key);
    };
    return obj
}

//
export const createFormData = (data) => {
    const formData = new FormData();
    buildFormData(data, formData)

    return formData;
}


const buildFormData = (data, formData, parentKey) => {

    if (data && typeof data === 'object' && !(data instanceof Date) && !(data instanceof File)) {
        Object.keys(data).forEach(key => {
            buildFormData(data[key], formData, parentKey ? `${parentKey}.${key}` : key);
        });
    } else {
        const value = data == null ? '' : data;

        formData.append(parentKey, value);
    }
}


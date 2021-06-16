

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

//クエリストリングを作成する。
export const createQueryString = (obj) => {
    //クエリストリングで検索条件を指定された場合。
    const urlParams = new URLSearchParams(obj);
    const queryString = urlParams.toString()

    return queryString ? `?${queryString}` : "";
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

//C#のString.Formatのように、{数値}の箇所を引数に置き換えた文字列を作成する。
export const formatString = function (format) {
    if(!format){
        return ""
    }
    
    var args = Array.prototype.slice.call(arguments, 1);
    return format.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
            ? args[number]
            : match
    })
}


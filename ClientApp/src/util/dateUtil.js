//日付操作
import { format as fnsFormat, isValid as fnsIsValid, startOfDay as fnsStartOfDay, setYear,
     setMonth, setDate, endOfMonth as fnsEndOfMonth, startOfMonth as fnsStartOfMonth, addMonths as fnsAddMonths, addHours as fnsAddHours,
    toDate as fnsToDate, differenceInHours as fnsDifferenceInHours} from 'date-fns'

import fnsParse from 'date-fns/parse'

export const mitTime = "1900-01-01T00:00:00"

//最小日付(1900-01-01T00:00:00)を取得する。
export const getMinDate = (dateString, afterFormat) => {
    return new Date(1900, 0, 1, 0, 0, 0);
}


//日付文字列を別のフォーマットの文字列に変更する。
export const convert = (dateString, afterFormat) => {
    if(!dateString){
        return "";
    }

    const dateObject = parse(dateString)
    const result = format(dateObject, afterFormat)
    return result;
}

const serverDtFormat = "yyyy-MM-dd'T'HH:mm:ss";

//日付オブジェクトを指定したフォーマットの文字列に変更する。
export const format = (dateObject, afterFormat) => {
    if(!isValid(dateObject)){
        return "";
    }

    afterFormat = afterFormat || "yyyy/MM/dd"
    const result = fnsFormat(dateObject, afterFormat)
    return result ?? "";
}


//日付オブジェクトを指定したフォーマットの文字列に変更する。
export const formatToFullDateTime = (dateObject, afterFormat) => {
    if(!isValid(dateObject)){
        return "";
    }

    afterFormat = afterFormat || serverDtFormat
    const result = fnsFormat(dateObject, afterFormat)
    return result;
}


//文字列から日付オブジェクトを作成する。デフォルトフォーマットは「yyyy-MM-dd'T'HH:mm:ss」
export const parse = (dateString, dateFormat) => {
    if(!dateString){
        return null;
    }
    
    dateFormat = dateFormat || serverDtFormat
    const result = fnsParse(dateString, dateFormat, new Date())
    return result;
}

//正しい日付形式かをチェックする。
export const isValid = (dateObject) => {
    if(!dateObject || typeof dateObject.getMonth !== 'function'){
        return false;
    }
    return fnsIsValid(dateObject)
}

//時間が00:00:00の日付データに変換する。
export const startOfDay = (dateObject) => {
    return fnsStartOfDay(dateObject)
}

//年月日を1900-01-01に設定する。時間のみのフィールドなどに利用。
export const setMinDate = (dateObject) => {
    if(!isValid(dateObject)){
        return dateObject;
    }

    let result = setYear(dateObject, 1900)
    result = setMonth(result, 0)
    result = setDate(result, 1)
    return result
}

//月初日を取得する。
export const startOfMonth = (dateObject) => {
    if(!isValid(dateObject)){
        return dateObject;
    }

    return fnsStartOfMonth(dateObject)
}

//月末日を取得する。
export const endOfMonth = (dateObject) => {
    if(!isValid(dateObject)){
        return dateObject;
    }

    return fnsEndOfMonth(dateObject)
}

//月を加算する。
export const addMonths = (dateObject, amount) => {
    if(!isValid(dateObject)){
        return dateObject;
    }

    return fnsAddMonths(dateObject, amount)
}

//時間を加算する。
export const addHours = (dateObject, amount) => {
    if(!isValid(dateObject)){
        return dateObject;
    }

    return fnsAddHours(dateObject, amount)
}

//日付もしくはミリ秒から新しいDateオブジェクトを作成する。
export const toDate = (dateOrMillisecond) => {
    return fnsToDate(dateOrMillisecond)
}

//日付同士の時間の差分をス取得する。
export const differenceInHours = (from, to) => {
    if(!isValid(from) || !isValid(to) ){
        return 0; //日付型ではない場合は0。
    }

    const result = fnsDifferenceInHours(to, from)
    return result
}
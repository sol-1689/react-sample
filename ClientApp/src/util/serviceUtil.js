import api from './api'
import { useDispatch } from "react-redux";

import {backdropActions} from '../global/stores/backdropSlice'
import {errorDialogActions} from '../global/stores/errorDialogSlice'
import store from '../global/stores/mainStore'
import * as constant from './constant'
import { createFormData } from './other'

//コンテキストパス
const contextPath = constant.contextPath;

//webpackのconfigファイル内のプラグイン(webpack.EnvironmentPlugin)内で定義している。
//本番環境では空文字になる想定。
//const apiUrlBase = process.env.API_URL || ""; //不要だった。

const postAndResult = async (apiPath, postObject) => {
    try{
        //urlにコンテキストパスを付与。
        const response = await api.post(`${contextPath}${apiPath}`, postObject);

        //401などでエラーの場合はresponseにはerrorが入っている。
        if(response.isApiError === true){
            if(response.handled !== true){
                store.dispatch(errorDialogActions.showMessage(response.message))
            }
            return {IsSuccess: false}
        }

        const result = response.data

        if(!result.IsSuccess){
            store.dispatch(errorDialogActions.showMessage(result.AllErrorMessage))
        }

        return result;
    }catch(err){
        store.dispatch(errorDialogActions.showMessage(err.toString()))
        //throw err
    }
}


//ファイルアップロードなどのfetchリクエストを発行する。
const fetchAndResult = async (apiPath, postObject) => {
    try{
        const formData = createFormData(postObject)

        //urlにコンテキストパスを付与。
        const response = await api.callFetch(`${contextPath}${apiPath}`, formData);

        if(response.ok){
            const result = await response.json()

            if(!result.IsSuccess){
                store.dispatch(errorDialogActions.showMessage(response.AllErrorMessage))
            }
            return result;
        }else{
            store.dispatch(errorDialogActions.showMessage(response.statusText))
            return {IsSuccess: false}
        }

    }catch(err){
        store.dispatch(errorDialogActions.showMessage(err.toString()))
        return {IsSuccess: false}
    }
}

//backdrop(クルクル)を出しつつアクションを実行する。
const actionWithBackdrop = async (action, message = "") => {
    try{
        store.dispatch(backdropActions.openWithMessage(message))
        const result = await action()
        return result
    }finally{
        store.dispatch(backdropActions.close())
    }
}

//処理がエラー時はエラーメッセージの表示を行う。
const actionWithShowError = async (action) => {
    try{
        const result = await action()
        return result
    }catch(err){
        store.dispatch(errorDialogActions.showMessage(err.toString()))
    }
}

export default {
    postAndResult, actionWithBackdrop, actionWithShowError
}

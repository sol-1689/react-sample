import axios from 'axios'
import * as constant from './constant'

// //async・await用に読み込み。
// import 'core-js/stable';
// import 'regenerator-runtime/runtime';

//const apiKey = "";

//AppParametersは、サーバー側の_SpaLayout.cshtmlにて設定している。
const csrfTokenName = "RequestVerificationToken"
const csrfToken = AppParameters.XsrfToken

//認証情報を保持したトークン。
const jwtToken = ""

//リクエスト時のヘッダーなど、オプションを指定する。
const config = {
    headers: {
        //API_KEY: apiKey,
        //aspcoreが規定するCsrfTokenのヘッダー名。
        [csrfTokenName]: csrfToken
    }
}

const baseAxios = axios.create(config)

baseAxios.interceptors.response.use(response => {
    return response;
}, error => {
    if(!error.response){
        console.log(`API通信エラー。response無し【${error}】`);
    }else if(error.response.status === 401){
        const redirectUrl = error.response.headers['redirecturl']; //ヘッダー名は全て小文字に変換されるので注意。
        error.handled = true;
        location.href = redirectUrl; //期限切れなどで認証されていなかった場合は、サーバーから渡されたログインURLへ遷移する。
    }else if(error.response.status === 403){
        alert("権限がありません。"); //通常はここに来ないので、普通のalertで十分かと。
        error.handled = true;
        location.href = constant.top_page_url; //権限が無い場合はトップページへ遷移する。
    }

    error.isApiError = true;
    return error;
})

const post = async (url, postObject) =>{
    console.log(url);
    return await baseAxios.post(url, postObject);
}

const callFetch = async (url, postObject) => {
    const response = await fetch(url, {
        method: 'POST',
        body: postObject,
        headers: {
            [csrfTokenName]: csrfToken
        },
    })
    return response
}

export default {
    post, callFetch
}
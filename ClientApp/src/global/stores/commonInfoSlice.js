
//messageDialog用のaction・reducer。
import React, {useEffect} from 'react'
import {useDispatch} from 'react-redux'


import {createSlice} from '@reduxjs/toolkit'

import commonService from '../service/commonService'

const initialState = {
    //loginUserInfo: { UserNo: null, Role: null, Auth: null, IsAdmin: false, DivisionNoList: [] },
}

const slice = createSlice({
    name: "commonInfo",
    initialState,
    reducers: {
        // setLoginUserInfo: (state, action) => {
        //     state.loginUserInfo = action.payload;
        // },
        
    }
})

export default slice.reducer
export const actions = slice.actions

// //ログインユーザー情報を取得
// export const getLoginUserInfo = () => async (dispatch, getState) => {
//     const result = await commonService.getLoginUserInfo()
//     dispatch(actions.setLoginUserInfo(result.Result || {}))
// }



// //共通情報を読み込むためのコンポーネント。useDispatchはProviderタグの範囲内で呼ばないとエラーになる為、App内では直接呼べない。
// export const LoginInfoLoader = () => {
//     const dispatch = useDispatch()

//     useEffect(() => {
//         dispatch(getLoginUserInfo())
//     }, [])

//     return (
//         <></>
//     )
// }


// //アプリで共通の値を取得するためのコンポーネント。
// export const AppInfoLoader = () => {
//     const dispatch = useDispatch()

//     useEffect(() => {
//         //AppParametersは、サーバー側の_SpaLayout.cshtmlにて設定している。

//         //コンテキストパス
//         dispatch(setContextPath(window.AppParameters.ContextPath))
//         //xsrfトークン
//         dispatch(setXsrfToken(window.AppParameters.XsrfToken))
//     }, [])

//     return(
//         <></>
//     )
// }
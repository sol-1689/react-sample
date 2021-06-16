//ユーザーメンテナンス用のaction・reducer。

import { createSlice } from "@reduxjs/toolkit";

import service from '../service/userMainteService'
import kubunService from '~/global/service/kubunService'

import { errorDialogActions } from '~/global/stores/errorDialogSlice' 

const searchInitial = {SearchUserName: "", SearchLoginId: "", SearchDivisionNo: "", SearchRole: "", ShowDelete: false}
const editInfoInitial = { UserNo: null, UserName: "", LoginId:"", PasswordStr: "", OwnerId:"", DivisionNoList:[], Role:"", IsDeleted: false}
export const initialState = {
    //区分値
    roleList: [],
    divisionList: [],

    //検索フォーム
    searchInfo: searchInitial,
    searchErrorMessages: [],

    //実際に検索されたパラメータ。検索されるごとに更新。
    currentSearchInfo: searchInitial,

    //検索結果
    searchResult: {TotalCount: 0, List:[]},
    sortInfoList: [], //{ColumnName:'', Order:'asc'},
    pagingInfo: {CurrentPage:0, RowCount:10},
    selectedUserNo: null,

    //登録フォーム
    originalInfo: editInfoInitial,
    editingInfo: editInfoInitial,
    registerErrorMessages: [],
    //isUpdate: true,
    //isEditing: false
}

const slice = createSlice({
    name: "userMainte",
    initialState: initialState,
    reducers: {
        //全情報を初期値に戻す。
        allClear:  (state, action) => {
            Object.keys(initialState).forEach(key => {
                state[key] = initialState[key]
            })
        },

        //区分値
        setRoleList: (state, action) => {
            state.roleList = action.payload;
        },
        setDivisionList: (state, action) => {
            state.divisionList = action.payload;
        },

        //検索フォーム
        setSearchInfo: (state, action)=> {
            state.searchInfo = action.payload;
        },

        //実際に検索した値を保持しておく
        setCurrentSearchInfo: (state, action) => {
            state.currentSearchInfo = {...action.payload};
        },

        setSearchErrorMessages: (state, action)=> {
            console.log(`setSearchErrorMessages ${action.payload}`)
            state.searchErrorMessages = action.payload;
        },

        //検索結果
        setSearchResult: (state, action) => {
            const searchResult = action.payload;
            //searchResult.DivisionNoList = searchResult.DivisionNoList.map(d => d + ""); //DivisionNoは数値を文字列に変換しておく。
            state.searchResult = searchResult;
        },

        setSortInfo: (state, action)=> {
            state.sortInfoList = action.payload;
        },

        setPagingInfo: (state, action)=> {
            state.pagingInfo = action.payload;
        },

        setCurrentPage: (state, action)=> {
            state.pagingInfo = {...state.pagingInfo, CurrentPage: action.payload}
        },

        setRowCount: (state, action)=> {
            state.pagingInfo = {...state.pagingInfo, RowCount: action.payload}
        },

        setSelectedUserNo: (state, action)=> {
            state.selectedUserNo = action.payload;
        },

        clearResult: (state, action) => {
            state.sortInfoList = initialState.sortInfoList
            state.pagingInfo = {...state.pagingInfo, CurrentPage:0}
            state.selectedUserNo = initialState.selectedUserNo
            //state.isUpdate = false
        },


        //登録フォーム
        setOriginalInfo: (state, action) => {
            state.originalInfo = action.payload;
        },

        //setEditingInfo: (state, action) => {
        //    state.editingInfo = action.payload;
        //},

        setRegisterErrorMessages: (state, action)=> {
            state.registerErrorMessages = action.payload;
        },
    }
})

export default slice.reducer
export const actions = slice.actions

//検索処理を行う。
//searchParamsは以下メンバーを想定。
//{ searchInfo, sortInfoList, pagingInfo }
export const searchList = (searchParams) => async (dispatch, getState) => {
    // jsでのnull合体演算子(C#の??)っぽいことをする場合は「||」を利用する。
    const searchInfo = searchParams?.searchInfo || getState().userMainte.currentSearchInfo
    const sortInfoList = searchParams?.sortInfoList || getState().userMainte.sortInfoList
    const pagingInfo = searchParams?.pagingInfo || getState().userMainte.pagingInfo

    //ソート情報は、サーバー側がリストで受け取るので形式を合わせている。
    const result = await service.search(searchInfo, sortInfoList, pagingInfo);
    if(result.IsSuccess){
        //コンポーネント外からdispatchする場合は、hooksは使えない(エラーになる)ので、直接storeからdispachを呼び出す。
        dispatch(actions.setSearchResult(result.Result))
        dispatch(actions.setCurrentSearchInfo(searchInfo))
    }else{
        dispatch(errorDialogActions.showMessage(result.AllErrorMessage))
        //dispatch(actions.setSearchErrorMessages(result.ErrorMessages))
    }
}

//役割一覧を取得
export const getRoleList = () => async (dispatch, getState) => {
    const result = await kubunService.getRoleList()
    dispatch(actions.setRoleList(result.Result || []))
}

//課一覧を取得
export const getDivisionList = () => async (dispatch, getState) => {
    const result = await kubunService.getDivisionList()
    dispatch(actions.setDivisionList(result.Result || []))
}



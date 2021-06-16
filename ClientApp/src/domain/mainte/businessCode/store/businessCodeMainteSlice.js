//事業コードメンテナンス用のaction・reducer。

import { createSlice } from "@reduxjs/toolkit";

import service from '../service/businessCodeMainteService'
import kubunService from '~/global/service/kubunService'

import { errorDialogActions } from '~/global/stores/errorDialogSlice' 

const searchInitial = {SelectedAccountingCode: "", SearchDebitBusinessCode: "", SearchDebitBusinessName: "", ShowDelete: false}
const editInfoInitial = { BusinessCodeNo: "", AccountingCodeName: "", AccountingCodeNo: "", DebitBusinessCode: "", DebitBusinessName:"", DebitAccountingItemCode: "", DebitAccountingAssistItemCode:"", DebitTaxCode:"", CreditBusinessCode:"", CreditAccountingItemCode:"", CreditAccountingAssistItemCode:"", CreditTaxCode:"", IsPaid: false, IsDeleted: false}
export const initialState = {
    //区分値
    accountingCodeList: [],

    //検索フォーム
    searchInfo: searchInitial,
    searchErrorMessages: [],
    //実際に検索されたパラメータ。検索されるごとに更新。
    currentSearchInfo: searchInitial,

    //検索結果
    searchResult: {TotalCount: 0, List:[]},
    sortInfoList: [], //{ColumnName:'', Order:'asc'},
    pagingInfo: {CurrentPage:0, RowCount:10},
    selectedBusinessCodeNo: null,

    //登録フォーム
    originalInfo: editInfoInitial,
    editingInfo: editInfoInitial,
    registerErrorMessages: [],
    //isUpdate: true,
    //isEditing: false
}

const slice = createSlice({
    name: "BusinessCodeMainte",
    initialState: initialState,
    reducers: {
        //全情報を初期値に戻す。
        allClear:  (state, action) => {
            Object.keys(initialState).forEach(key => {
                state[key] = initialState[key]
            })
        },

        //区分値
        setAccountingCodeList: (state, action) => {
            state.accountingCodeList = action.payload;
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
            state.searchResult = action.payload;
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

        setSelectedBusinessCodeNo: (state, action)=> {
            state.selectedBusinessCodeNo = action.payload;
        },

        clearResult: (state, action) => {
            state.sortInfoList = initialState.sortInfoList
            state.pagingInfo = {...state.pagingInfo, CurrentPage:0}
            state.selectedBusinessCodeNo = initialState.selectedBusinessCodeNo
        },


        //登録フォーム
        setOriginalInfo: (state, action) => {
            state.originalInfo = action.payload;
        },

        setEditingInfo: (state, action) => {
            state.editingInfo = action.payload;
        },

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
    const searchInfo = searchParams?.searchInfo || getState().businessCodeMainte.currentSearchInfo
    const sortInfoList = searchParams?.sortInfoList || getState().businessCodeMainte.sortInfoList
    const pagingInfo = searchParams?.pagingInfo || getState().businessCodeMainte.pagingInfo

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

//会計コード一覧を取得
export const getAccountingCodeList = () => async (dispatch, getState) => {
    const result = await kubunService.getAccountingCodeList()
    dispatch(actions.setAccountingCodeList(result.Result || []))
}



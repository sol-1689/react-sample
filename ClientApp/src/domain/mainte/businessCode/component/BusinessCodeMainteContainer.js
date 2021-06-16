import React, {useState, useEffect, useLayoutEffect } from 'react'
import {useDispatch, useSelector } from 'react-redux'

//material-ui用
import { makeStyles } from '@mui/styles';
import { Typography, Button } from '@mui/material';
import { Grid } from '@mui/material';

//ルーティング用
import { BrowserRouter as Router, Switch, Route, Link, useParams, useRouteMatch, useHistory  } from "react-router-dom";


import { actions, initialState, getAccountingCodeList, searchList } from '../store/businessCodeMainteSlice'

import RegisterForm from './RegisterForm'
import SearchForm from './SearchForm'
import SearchResult from './SearchResult'
import PageTitle from '~/global/component/PageTitle'

const useStyles = makeStyles(theme => ({
    newButtonContainer:{
        margin: theme.spacing(1, 0)
    }
}))

//事業コードメンテ画面
const BusinessCodeMainteContainer = () => {
    const dispatch = useDispatch()

    //ルーティング用
    //親でマッチしたurlを取得。(ここの場合/view/mainte/businessCode)
    const { path, url} = useRouteMatch()

    const title = "事業コード"

    //マウント時(Domに描画時)に一度だけ区分値を取得する。
    //（2つ目の引数の空配列でDidMountのタイミングでの実行となる。）
    useLayoutEffect(() => {
        dispatch(actions.allClear()) //storeをクリア
        dispatch(getAccountingCodeList())

        document.title = title //タイトルもここで変更
    }, [])

    //ダイアログやbackdropはapp.jsに定義している。
    return (
        <Switch>
            {/* 親のurlと同一の場合は検索画面を表示。 */}
            <Route exact path={path}>
                <PageTitle title={title}/>
                <SearchForm/>
                <NewButtonArea/>
                <SearchResult/>
            </Route>

            {/* /edit/:BusinessCodeNoもしくは/newの場合は登録画面を表示。(BusinessCodeNoは新規作成の場合は来ない。そのようなオプション値を受ける場合は、?を付けたパラメータ名を指定する。) 
                親のRouteでexactを指定しないこと。(完全一致にpathの後ろがあると404扱いになってしまう。)
            */}
            <Route path={`${path}/:type/:BusinessCodeNo?`}>
                <RegisterForm/>
            </Route>
        </Switch>
    )
}

//新規登録ボタンの箇所
const NewButtonArea = () => {
    const classes = useStyles()
    const dispatch = useDispatch()

    //ルーティング用
    const { path, url} = useRouteMatch()
    const history = useHistory();

    //新規登録時のイベント
    const onNew = (e) => {
        e.preventDefault();
        dispatch(actions.setOriginalInfo(initialState.originalInfo)) //新規登録時は空オブジェクトを渡す
        history.push(`${url}/new`) //新規登録画面へ遷移
        //dispatch(actions.setNewState())
    }

    return (
        <Grid container className={classes.newButtonContainer}>
            <Grid item xs={12} md={2}>
                {/* 新規登録画面へ遷移 */}
                <Button
                    type="button"
                    variant="contained"
                    color="primary"
                    size="medium"
                    onClick={onNew}
                    className={classes.submit}>
                    新規登録
                </Button>
            </Grid>
        </Grid>
    )
}


export default BusinessCodeMainteContainer 
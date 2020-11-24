import React, {useState, useEffect } from 'react'
import {useDispatch, useSelector } from 'react-redux'

//material-ui用
import Typography from '@material-ui/core/Typography';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import { makeStyles } from '@material-ui/core/styles';

//ルーティング用
import { BrowserRouter as Router, Switch, Route,Link, useParams, useRouteMatch, useHistory  } from "react-router-dom";


import { actions, initialState, getRoleList, getDivisionList, searchList } from '../store/userMainteSlice'

import RegisterForm from './RegisterForm'
import SearchForm from './SearchForm'
import SearchResult from './SearchResult'
import PageTitle from '~/global/component/PageTitle'

const useStyles = makeStyles(theme => ({
    newButtonContainer:{
        margin: theme.spacing(1, 0)
    }
}))

//ユーザーメンテ画面
const UserMainteContainer = () => {
    const dispatch = useDispatch()

    //ルーティング用
    //親でマッチしたurlを取得。(ここの場合/view/mainte/user)
    const { path, url} = useRouteMatch()

    const title = "ユーザー"

    //マウント時(Domに描画時)に一度だけ区分値を取得する。
    //（2つ目の引数の空配列でDidMountのタイミングでの実行となる。）
    useEffect(() => {
        dispatch(actions.allClear()) //storeをクリア

        //区分値の取得
        dispatch(getRoleList())
        dispatch(getDivisionList())

        //最初に検索をする。
        dispatch(searchList())

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

            {/* /edit/:UserNoもしくは/newの場合は登録画面を表示。(UserNoは新規作成の場合は来ない。そのようなオプション値を受ける場合は、?を付けたパラメータ名を指定する。) 
                親のRouteでexactを指定しないこと。(完全一致にpathの後ろがあると404扱いになってしまう。)
            */}
            <Route path={`${path}/:type/:userNo?`} render={({ match }) =>
                <RegisterForm userNo={match.params.userNo} type={match.params.type} baseUrl={path}/>
            } />
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
                    //fullWidth
                    size="medium"
                    onClick={onNew}
                    >
                    新規登録
                </Button>
            </Grid>
        </Grid>
    )
}


export default UserMainteContainer 
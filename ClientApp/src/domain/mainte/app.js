//IE11用のPolyfill 。Promiseなどを動作可能とする。
//webpackのentryに指定しているルートコンポーネントでインポートする。
//必ず最上部で定義すること!!
import 'core-js/stable';
import 'regenerator-runtime/runtime';
import 'whatwg-fetch'

import React, { useState } from 'react'
import ReactDom from 'react-dom'
import { BrowserRouter as Router, Route, Link, Redirect, withRouter } from 'react-router-dom'

import Container from '@material-ui/core/Container';
import { makeStyles } from '@material-ui/core/styles';

import LayoutTemplate from '~/global/component/LayoutTemplate'
//LayoutTemplateの中でstoreを呼び出している。
//ストアを使用するコンポーネントのimport文は、必ずこの下に記述する事。
import * as constant from '~/util/constant'
import MyMenu from '~/global/component/menu'
import MainteTop from './top/component/MainteTop'
import UserMainteContainer from './user/component/UserMainteContainer'
import BusinessCodeMainteContainer from './businessCode/component/BusinessCodeMainteContainer'


//async・await用に読み込み。
// import 'core-js/stable';
// import 'regenerator-runtime/runtime';

const useStyle = makeStyles(theme => ({
    main_container: {
        marginTop: theme.spacing(2)
    }
}))

const MainteApp = () => {
    const classes = useStyle()

    return (
        <LayoutTemplate>
            <Router>
                <MyMenu></MyMenu>

                <Container component='div' maxWidth='xl' className={classes.main_container} >
                    
                    <Route exact path={`${constant.mainte_view_base}/`} render={() =>

                        // 特定の画面へリダイレクトさせたい場合。
                        <Redirect to={`${constant.mainte_view_base}/user`} />
                    } />

                    <Route path={`${constant.mainte_view_base}/user`} render={({ match }) =>
                        <UserMainteContainer></UserMainteContainer>
                    } />

                    <Route path={`${constant.mainte_view_base}/businessCode`} render={({ match }) =>
                        <BusinessCodeMainteContainer></BusinessCodeMainteContainer>
                    } />


                    {/* <Route path={`${constant.mainte_view_base}/`} render={({ match }) => {
                        window.location.reload()
                        return (<></>)
                    }
                    } /> */}
                </Container>
            </Router>
        </LayoutTemplate>
    )
}


ReactDom.render(

    <MainteApp />,
    document.getElementById('app')
)

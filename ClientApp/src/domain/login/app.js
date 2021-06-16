//IE11用のPolyfill 。Promiseなどを動作可能とする。
//webpackのentryに指定しているルートコンポーネントでインポートする。
//必ず最上部で定義すること!!
import 'core-js/stable';
import 'regenerator-runtime/runtime';

import CssBaseline from '@mui/material/CssBaseline';

import React from 'react';
import ReactDom from 'react-dom';

import { Provider } from 'react-redux'
import store from '~/global/stores/mainStore'

import { ThemeProvider } from '@mui/material/styles';
import { getDefaultTheme } from '~/util/designUtil'

import LoginFormContainer from './component/LoginForm';
import DefaultBackDrop from '~/global/component/backdrop'


//デフォルトデザイン
const theme = getDefaultTheme();

const LoginApp = () => {
    return (
        <div>
            <Provider store={store}>
                <ThemeProvider theme={theme}>
                    <CssBaseline />
                    <LoginFormContainer />
                    <DefaultBackDrop></DefaultBackDrop>
                </ThemeProvider>
            </Provider>
        </div>
    )
}


ReactDom.render(
    <LoginApp />,
    document.getElementById('app')
)

import React from 'react'

import { Provider } from 'react-redux'
import store from '~/global/stores/mainStore'
//ストアを使用するコンポーネントのimport文は、必ずこの下に記述する事。

import * as constant from '~/util/constant'
import { formatToFullDateTime } from '~/util/dateUtil'
import { YesNoDialog, MessageDialog, ErrorDialog } from '~/global/component/dialogs'
import DefaultBackDrop from '~/global/component/backdrop'
import { getDefaultTheme } from '~/util/designUtil'

import CssBaseline from '@mui/material/CssBaseline';
import { ThemeProvider } from '@mui/material/styles';

//DatePicker用設定
import AdapterDateFns from '@mui/lab/AdapterDateFns';
import LocalizationProvider from '@mui/lab/LocalizationProvider';

import { ja } from 'date-fns/locale'

//デフォルトデザイン
const theme = getDefaultTheme();

//デフォルトだと、Post時にDateオブジェクトは勝手にUTCに変換された後に文字列にされる。
//(axiosが内部で利用しているJSON.stringify()がそういう挙動の為。)
//JSON.stringify()は、Dateオブジェクトが持つtoJSON()の戻り値を利用している。
//そのままのタイムゾーンで文字列に変換するようtoJSON()を変更する。
Date.prototype.toJSON = function () {
    return this.getFullYear() + '-' + ('0' + (this.getMonth() + 1)).slice(-2) + '-' + ('0' + this.getDate()).slice(-2) + 'T' +
        ('0' + this.getHours()).slice(-2) + ':' + ('0' + this.getMinutes()).slice(-2) + ':' + ('0' + this.getSeconds()).slice(-2) + '.000Z';
}


//全てのページのテンプレートを設定する。(csのLayout.cshtml的なもの)
const LayoutTemplate = ({ children }) => {
    return (
        <>
            <CssBaseline />
            <Provider store={store}>
                <ThemeProvider theme={theme}>
                    <LocalizationProvider dateAdapter={AdapterDateFns} locale={ja}>

                        {children}

                        <YesNoDialog></YesNoDialog>
                        <MessageDialog></MessageDialog>
                        <ErrorDialog></ErrorDialog>
                        <DefaultBackDrop></DefaultBackDrop>
                    </LocalizationProvider>
                </ThemeProvider>
            </Provider>
        </>
    )
}


export default LayoutTemplate
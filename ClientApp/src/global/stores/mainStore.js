//メインとなるstore
import {combineReducers} from 'redux'
import {configureStore, getDefaultMiddleware} from '@reduxjs/toolkit'

import commonInfoReducer from './commonInfoSlice'
import backdropReducer from './backdropSlice'
import yesNoDialogReducer from './yesNoDialogSlice'
import messageDialogReducer from './messageDialogSlice'
import errorDialogReducer from './errorDialogSlice'
import userMainteReducer from '../../domain/mainte/user/store/userMainteSlice'
import businessCodeMainteReducer from '../../domain/mainte/businessCode/store/businessCodeMainteSlice'


const reducer = {
    commonInfo: commonInfoReducer,
    backdrop: backdropReducer,
    yesNoDialog: yesNoDialogReducer,
    messageDialog: messageDialogReducer,
    errorDialog: errorDialogReducer,
    userMainte: userMainteReducer,
    businessCodeMainte: businessCodeMainteReducer,
    
}

const store = configureStore({
    reducer: reducer,
    middleware: getDefaultMiddleware({
        serializableCheck: false //これをfalseにしないと、functionをstoreに登録する際にエラーになってしまう。
    })
})

export default store
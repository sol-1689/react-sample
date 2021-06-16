
//messageDialog用のaction・reducer。
import {createSlice} from '@reduxjs/toolkit'

const initialState = {
    open: false, 
    title: "エラー", 
    message: "エラーが発生しました。", 
    eventWhenOK: () => {}
}

const slice = createSlice({
    name: "errorDialog",
    initialState,
    reducers: {
        isOpen: (state, action) => {
            state.open = action.payload
        },
        setTitle: (state, action) => {
            state.title = action.payload
        },
        setMessage: (state, action) => {
            state.message = action.payload
        },
        setEventWhenOK: (state, action) => {
            state.eventWhenOK = action.payload
        },
        setDialogInfo: (state, action) => {
            return action.payload
        },

        //タイトル等は変えずにメッセージだけを表示する場合に使用。
        showMessage: (state, action) => {
            //state.message = action.payload || "エラーが発生しました。"

            if(!!action.payload){
                state.title = initialState.title
                state.eventWhenOK = initialState.eventWhenOK

                state.message = action.payload
                state.open = true
            }
        }
    }
})

export default slice.reducer
export const errorDialogActions = slice.actions
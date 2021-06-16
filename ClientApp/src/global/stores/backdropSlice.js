//backdrop用のaction・reducer。

import {createSlice} from '@reduxjs/toolkit'

const initialState = {
    open: false,
    message: ""
}

const slice = createSlice({
    name: "backdrop",
    initialState,
    reducers: {
        isOpen: (state, action) => {
            state.open = action.payload
        },

        //メッセージ付きで表示する。
        openWithMessage: (state, action) => {
            state.message = action.payload
            state.open = true
        },

        //閉じる。
        close: (state, action) => {
            state.message = initialState.message
            state.open = false
        },

    }
})

export default slice.reducer
export const backdropActions = slice.actions
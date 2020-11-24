
//YesNoDialog用のaction・reducer。
import {createSlice} from '@reduxjs/toolkit'

const initialState = {
    open: false, 
    title: "", 
    message: "", 
    eventWhenOK: () => {},
    eventWhenNo: () => {}
}

const slice = createSlice({
    name: "yesNoDialog",
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
        setEventWhenNo: (state, action) => {
            state.eventWhenNo = action.payload
        },
        setDialogInfo: (state, action) => {
            return action.payload
        }
    }
})

export default slice.reducer
export const yesNoDialogActions = slice.actions
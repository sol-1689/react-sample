// //Validation用
// //import * as yup from 'yup';
// import React from 'react';
// //material-ui用
// import { makeStyles } from '@mui/styles'
// import { Box } from '@mui/material'

//オブジェクトをvalidateし、エラーがあればエラーオブジェクトを返す。schemeはyupの利用を想定。
export const validate = (obj, scheme) => {
    try{
        scheme.validateSync(obj, {abortEarly: false})
        return {};
    }catch(e){
        const errorObj = {};
        const errorFields = e.inner;
        errorFields.forEach(each => {
            errorObj[each.path] = {message: each.message}
        })

        return errorObj;
    }
}

//エラーオブジェクト内にエラーがなければtrueを返す。
export const hasNoError = (errors) => {
    let result = true;

    Object.keys(errors).forEach(key => {
        if(!!errors[key]){
            result = false;
        }
    })
    return result;
}


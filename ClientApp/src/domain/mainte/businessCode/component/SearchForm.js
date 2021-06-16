import React, { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

import { makeStyles } from '@mui/styles';
import { Button, TextField, Typography, InputLabel, Checkbox, FormControl, FormControlLabel, FormHelperText, Select, Tooltip } from '@mui/material';
import { Paper, Container, Box, Grid } from '@mui/material';

import { actions, searchList, initialState } from '../store/businessCodeMainteSlice'
import { DefaultSelectList } from '~/global/component/selectList'

const useStyles = makeStyles(theme => ({
    root: {
        padding: theme.spacing(1)
    }
}))

//検索部分
const SearchForm = () => {
    const dispatch = useDispatch()
    const classes = useStyles()

    const searchInfo = useSelector(state => state.businessCodeMainte.searchInfo)

    //区分値
    const accountingCodeList = useSelector(state => state.businessCodeMainte.accountingCodeList)

    //マウント時(Dom描画後)に一度検索を行う。
    //（2つ目の引数の空配列でDidMountのタイミングでの実行となる。）
    useEffect(() => {
        //最初に検索をする。
        dispatch(searchList())
    }, [])

    //検索処理
    const onSubmit = async (e) => {
        e.preventDefault(); //デフォルトのsubmit処理を中止。

        //ページごとの表示件数はそのままで検索。
        dispatch(actions.clearResult()) //ソートやページングを初期化

        dispatch(searchList({ searchInfo }))

    }

    return (
        <Paper elevation={1} className={classes.root}>
            <form onSubmit={onSubmit}>

                <Grid container spacing={1} alignItems="center">
                    <Grid item xs={12} md={3}>
                        <DefaultSelectList
                            valueTextList={accountingCodeList}
                            showNoSelect={true}
                            fullWidth
                            label="会計コード"
                            labelid="Lbl_AccountingCode"
                            name="SelectedAccountingCode"
                            value={searchInfo.SelectedAccountingCode}
                            onChange={(e) => dispatch(actions.setSearchInfo({ ...searchInfo, SelectedAccountingCode: e.target.value }))}
                        />
                    </Grid>

                    <Grid item xs={12} md={3}>
                        <TextField
                            name="SearchText"
                            value={searchInfo.SearchDebitBusinessCode}
                            label="借方事業コード"
                            //variant="outlined"
                            //margin="normal"
                            //size="small"
                            fullWidth
                            autoFocus
                            onChange={(e) => dispatch(actions.setSearchInfo({ ...searchInfo, SearchDebitBusinessCode: e.target.value }))}
                        />
                    </Grid>

                    <Grid item xs={12} md={3}>
                        <TextField
                            name="SearchText"
                            value={searchInfo.SearchDebitBusinessName}
                            label="借方事業名"
                            //variant="outlined"
                            //margin="normal"
                            //size="small"
                            fullWidth
                            onChange={(e) => dispatch(actions.setSearchInfo({ ...searchInfo, SearchDebitBusinessName: e.target.value }))}
                        />
                    </Grid>

                    <Grid item xs={12} md={2}>
                        <FormControlLabel
                            value="end"
                            control={<Checkbox color="primary" checked={searchInfo.ShowDelete}
                                onChange={(e) => dispatch(actions.setSearchInfo({ ...searchInfo, ShowDelete: e.target.checked }))} />}
                            label="削除済みを検索"
                            labelPlacement="end"
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={1} direction="row" justifyContent="center" alignItems="center" sx={{ mt: 1 }}>
                    <Grid item xs={12} md={2}>
                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            color="secondary">
                            検索
                        </Button>
                    </Grid>
                </Grid>
            </form>
        </Paper>

    )
}

export default SearchForm
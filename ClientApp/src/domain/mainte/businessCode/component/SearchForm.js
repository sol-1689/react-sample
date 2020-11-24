import React, { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

import TextField from '@material-ui/core/TextField';
import Checkbox from '@material-ui/core/Checkbox';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import FormControl from '@material-ui/core/FormControl';
import InputLabel from '@material-ui/core/InputLabel';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import { Paper } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import { actions, searchList, initialState } from '../store/businessCodeMainteSlice'
import { DefaultSelectList } from '~/global/component/selectList'

const useStyles = makeStyles(theme => ({
    root:{
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

    useEffect(() => {
        console.log("SearchFormのuseEffect")
    })

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

                <Grid container spacing={2} alignItems="center">
                    <Grid item xs={12} md={2}>
                        <FormControl className={classes.formControl}>
                            <InputLabel shrink id="Lbl_AccountingCode">会計コード</InputLabel>
                            <DefaultSelectList
                                valueTextList={accountingCodeList}
                                showNoSelect={true}
                                labelid="Lbl_AccountingCode"
                                name="SelectedAccountingCode"
                                value={searchInfo.SelectedAccountingCode}
                                onChange={(e) => dispatch(actions.setSearchInfo({ ...searchInfo, SelectedAccountingCode: e.target.value }))}
                                displayEmpty={true}
                            />
                        </FormControl>
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
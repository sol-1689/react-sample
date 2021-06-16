import React, {useState, useEffect } from 'react'
import {useDispatch, useSelector } from 'react-redux'

import { makeStyles } from '@material-ui/core/styles';
import { Button, TextField, Typography, InputLabel, Checkbox, FormControl, FormControlLabel, FormHelperText, Select, Tooltip } from '@material-ui/core';
import { Paper, Container, Box, Grid } from '@material-ui/core';

import { actions, searchList, initialState } from '../store/userMainteSlice' 
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

    //検索条件
    const searchInfo = useSelector(state => state.userMainte.searchInfo)
    //区分値
    const roleList = useSelector(state => state.userMainte.roleList)
    const divisionList = useSelector(state => state.userMainte.divisionList)

    //マウント時(Dom描画後)に一度検索を行う。
    //（2つ目の引数の空配列でDidMountのタイミングでの実行となる。）
    useEffect(() => {
        //最初に検索をする。
        dispatch(searchList())
    }, [])

     //テキスト変更時のイベント
     const handleTextChange = (event) => {
        dispatch(actions.setSearchInfo({...searchInfo, [event.target.name]: event.target.value}))
    };

    //検索処理
    const onSubmit = async (e) => {
        e.preventDefault(); //デフォルトのsubmit処理を中止。
       
        //ページごとの表示件数はそのままで検索。
        dispatch(actions.clearResult()) //ソートやページングを初期化
        dispatch(searchList({searchInfo}))
        //const result = await service.search(searchInfo, null, {...pagingInfo, CurrentPage: 0})
        //dispatch(actions.setNoEditState()) //検索後に編集不可状態に遷移。
    }

    return (
        <Paper elevation={1} className={classes.root}>
            <form onSubmit={onSubmit}>

                <Grid container spacing={2} alignItems="center">

                    <Grid item xs={12} md={2}>
                        <TextField
                            name="SearchUserName"
                            value={searchInfo.SearchUserName}
                            label="ユーザー名"
                            //variant="outlined"
                            //margin="normal"
                            //size="small"
                            fullWidth
                            onChange={handleTextChange}
                        />
                    </Grid>

                    <Grid item xs={12} md={2}>
                        <TextField
                            name="SearchLoginId"
                            value={searchInfo.SearchLoginId}
                            label="アカウント"
                            //variant="outlined"
                            //margin="normal"
                            //size="small"
                            fullWidth
                            onChange={handleTextChange}
                        />
                    </Grid>

                    <Grid item xs={12} md={2}>
                        <FormControl className={classes.formControl} style={{minWidth: "12em"}}>
                            <InputLabel shrink id="Lbl_SearchDivisionNo"> 担当</InputLabel>
                            <DefaultSelectList
                                valueTextList={divisionList}
                                showNoSelect={true}
                                labelid="Lbl_SearchDivisionNo"
                                name="SearchDivisionNo"
                                value={searchInfo.SearchDivisionNo}
                                onChange={handleTextChange}
                                displayEmpty={true}
                            />
                        </FormControl>
                    </Grid>

                    <Grid item xs={12} md={2}>
                        <FormControl className={classes.formControl} style={{minWidth: "12em"}}>
                            <InputLabel shrink id="Lbl_SearchRole"> 役割</InputLabel>
                            <DefaultSelectList
                                valueTextList={roleList}
                                showNoSelect={true}
                                labelid="Lbl_SearchRole"
                                name="SearchRole"
                                value={searchInfo.SearchRole}
                                onChange={handleTextChange}
                                displayEmpty={true}
                            />
                        </FormControl>
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
                            size="medium"
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
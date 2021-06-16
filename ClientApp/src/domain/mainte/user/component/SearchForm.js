import React, { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

import { makeStyles } from '@mui/styles';
import { Button, TextField, Typography, InputLabel, Checkbox, FormControl, FormControlLabel, FormHelperText, Select, Tooltip } from '@mui/material';
import { Paper, Container, Box, Grid } from '@mui/material';

import { DatePicker, TimePicker } from '@mui/lab';
import { isValid, startOfDay, addHours, convert, parse, format, formatToFullDateTime, setMinDate } from '~/util/dateUtil'

import { actions, searchList, initialState } from '../store/userMainteSlice'
import { DefaultSelectList } from '~/global/component/selectList'

const useStyles = makeStyles(theme => ({
    root: {
        padding: theme.spacing(1)
    }
}))

// class ExtendedUtils extends DateFnsUtils {
//     getCalendarHeaderText(date) {
//         return fnsFormat(date, "yyyy MMM", { locale: this.locale });
//     }
//     getDatePickerHeaderText(date) {
//         return fnsFormat(date, "MMMd日", { locale: this.locale });
//     }
// }

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
        dispatch(actions.setSearchInfo({ ...searchInfo, [event.target.name]: event.target.value }))
    };

    const handleDateChange = (name) => (d) => {
        // 時間を0に。
        let selectedDate = d && startOfDay(d)
        let valueObj = { SearchCreateDate: selectedDate }

        // let timeObj = null
        // if (selectedDate) {
        //     timeObj = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), selectedDate.getDate(),
        //         searchInfo.SearchCreateDate ? searchInfo.SearchCreateDate.getHours() : 0,
        //         searchInfo.SearchCreateDate ? searchInfo.SearchCreateDate.getMinutes() : 0, 0)
        //     const hourLater = addHours(new Date(), 1)
        //     if (timeObj < hourLater) {
        //         hourLater.setMinutes(Math.floor(hourLater.getMinutes() / 5) * 5)
        //         selectedDate = timeObj = hourLater
        //     }
        // }

        dispatch(actions.setSearchInfo({ ...searchInfo, ...valueObj }))
    }

    //検索処理
    const onSubmit = async (e) => {
        e.preventDefault(); //デフォルトのsubmit処理を中止。

        //ページごとの表示件数はそのままで検索。
        dispatch(actions.clearResult()) //ソートやページングを初期化
        dispatch(searchList({ searchInfo }))
        //const result = await service.search(searchInfo, null, {...pagingInfo, CurrentPage: 0})
        //dispatch(actions.setNoEditState()) //検索後に編集不可状態に遷移。
    }

    return (
        <Paper elevation={1} className={classes.root}>
            <form onSubmit={onSubmit}>

                <Grid container spacing={1} alignItems="center">

                    <Grid item xs={12} md={3}>
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

                    <Grid item xs={12} md={3}>
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

                    <Grid item xs={12} md={3}>
                        <DefaultSelectList
                            valueTextList={divisionList}
                            showNoSelect={true}
                            fullWidth
                            label="課"
                            labelid="Lbl_SearchDivisionNo"
                            name="SearchDivisionNo"
                            value={searchInfo.SearchDivisionNo}
                            onChange={handleTextChange}
                        />
                    </Grid>

                    <Grid item xs={12} md={3}>
                        <DefaultSelectList
                            valueTextList={roleList}
                            showNoSelect={true}
                            fullWidth
                            label="役割"
                            labelid="Lbl_SearchRole"
                            name="SearchRole"
                            value={searchInfo.SearchRole}
                            onChange={handleTextChange}
                        />
                    </Grid>

                    <Grid item xs={12} md={3}>
                        <DatePicker
                            name="SearchCreateDate"
                            inputFormat="yyyy/MM/dd"
                            mask="____/__/__"
                            views={["day"]}
                            value={searchInfo.SearchCreateDate}
                            onChange={handleDateChange("SearchCreateDate")}
                            clearable
                            //disablePast
                            //autoOk
                            // className={classes.date}
                            // okLabel="決定"
                            // cancelLabel="キャンセル"
                            // clearLabel="クリア"
                            renderInput={props => <TextField {...props} label="作成日" />}
                        />
                    </Grid>

                    <Grid item xs={12} md={3}>
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
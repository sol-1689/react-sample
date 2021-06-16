import React, {useState, useEffect } from 'react'
import {useDispatch, useSelector } from 'react-redux'

import { withStyles, makeStyles }  from '@material-ui/core/styles';
import { Paper, Container, Box, Typography, Tooltip } from '@material-ui/core';
import { Table, TableBody, TableRow, TableContainer, TablePagination } from '@material-ui/core';

import IconButton from '@material-ui/core/IconButton';
//import DeleteIcon from '@material-ui/icons/Delete';
import EditIcon from '@material-ui/icons/Edit';

//ルーティング用
import { BrowserRouter as Router, Switch, Route,Link,useParams, useRouteMatch, useHistory } from "react-router-dom";

import { yesNoDialogActions } from '~/global/stores/yesNoDialogSlice'
import { messageDialogActions } from '~/global/stores/messageDialogSlice'

import { ScrollableTableContainer, DefaultTableHeader, StyledTableRow, StyledTableCell, NoBorderCell } from '~/global/component/Table'
import { actions, searchList } from '../store/userMainteSlice' 
import service from '../service/userMainteService'


const useStyles = makeStyles(theme => ({
    root: {
        width: '100%',
        marginTop: theme.spacing(1),
        padding: theme.spacing(1)
    },
    tableContainer: {
        width: '100%',
        marginBottom: theme.spacing(1),
        overflow: "auto",
        height:"58vh"
    },
    table:{
        minWidth: 750,
        tableLayout: "fixed"
    }
}))


const headCells = [
    {id: 'edit',  label: '編集', disablePadding: true, ignoreSort: true, style:{width: "96px"}},
    {id: 'UserName', numeric: false, disablePadding: true, label: 'ユーザー名', style:{width: "240px"}},
    {id: 'LoginId', numeric: false, disablePadding: true, label: 'ログインID', style:{width: "240px"}},
    {id: 'DivisionNoStr', numeric: false, disablePadding: true, label: '課', style:{width: "auto"}},
    {id: 'Role', numeric: false, disablePadding: true, label: '役割', style:{width: "240px"}},
    //{id: 'delete',  label: '削除', disablePadding: true, ignoreSort: true}
]


//検索結果
//const SearchResult = ({searchedList, totalCount, onSearch, onDelete, onSelectedRow }) => {
const SearchResult = () => {
    const classes = useStyles()
    const dispatch = useDispatch()

    const sortInfoList = useSelector(state => state.userMainte.sortInfoList)
    const sortInfo = { ColumnName: sortInfoList.map(m => m.ColumnName).join(","), Order: sortInfoList.find(m => true)?.Order ?? "asc" }
    const pagingInfo = useSelector(state => state.userMainte.pagingInfo)
    const searchResult = useSelector(state => state.userMainte.searchResult)
    const selectedUserNo = useSelector(state => state.userMainte.selectedUserNo)

    //ルーティング用
    //親でマッチしたurlを取得。(ここの場合/view/mainte/user)
    const {path, url} = useRouteMatch()
    const history = useHistory()

    //行選択時の処理
    const handleRowClick = (userNo, event) => {
        dispatch(actions.setSelectedUserNo(userNo))

        //const selectedInfo = searchResult.List.find(each => each.UserNo === userNo)
        //dispatch(actions.setUpdateState(selectedInfo)) //選択した行を登録フォームに反映。
    }

    //編集ボタン押下時の処理
    const handleEdit = userNo => (event) => {
        event.stopPropagation(); //イベントのバブリングをキャンセル。これをしないと行のonclickが呼ばれてしまう。

        dispatch(actions.setSelectedUserNo(userNo))
        //const selectedInfo = searchResult.List.find(each => each.UserNo === userNo)
        //dispatch(actions.setOriginalInfo(selectedInfo)) // 選択した行を登録フォームに反映。 →詳細画面でサーバから詳細情報を取得するよう変更。

        history.push(`${url}/edit/${userNo}`) // 編集画面へ遷移
    }

    //削除時のイベント
    // const handleDelete = id => (event) => {
    //     event.stopPropagation(); //イベントのバブリングをキャンセル。これをしないと行のonclickが呼ばれてしまう。

    //     dispatch(yesNoDialogActions.setDialogInfo({
    //         open: true, 
    //         title: "削除", 
    //         message: "削除してよろしいですか？", 
    //         eventWhenOK: async () => {
    //             const deleteResult = await service.deleteFunc({UserNo: id, IsDeleted: true})
    //             if(deleteResult.IsSuccess){
    //                 //削除後に再検索する。
    //                 dispatch(searchList({sortInfo, pagingInfo}))
    //                 dispatch(messageDialogActions.setDialogInfo({open: true, title: "完了", message: `削除が完了しました。`}));
    //             }
    //         }
    //     }));
    // }

    //現在のページ変更時の処理
    const handleChangePage = async (event, newPage) => {
        const _pagingInfo = {...pagingInfo, CurrentPage:newPage}
        dispatch(actions.setPagingInfo(_pagingInfo))

        dispatch(searchList({ pagingInfo: _pagingInfo }))
    }

    //１ページ毎の表示件数変更処理
    const handleChangeRowsPerPage = async (event) => {
        const _pagingInfo = {CurrentPage:0, RowCount: parseInt(event.target.value, 10)}
        dispatch(actions.setPagingInfo(_pagingInfo))
        dispatch(searchList({ pagingInfo: _pagingInfo }))
    }

    //ソート時のイベント
    const handleSort = async (name, event)=> {
        const isDesc = sortInfo?.ColumnName === name && sortInfo?.Order === 'desc';
        const order = isDesc ? 'asc': 'desc';

        const _sortInfoList = name.split(',').map(m => { return {ColumnName: m, Order: order} })
        dispatch(actions.setSortInfo(_sortInfoList))

        dispatch(searchList({ sortInfoList: _sortInfoList }))
    }

    //空行の数
    const emptyRows = pagingInfo.RowCount - searchResult.List.length

    return (
        <Paper elevation={2} className={classes.root}>
                <TableContainer className={classes.tableContainer}>
                    <Table
                     className={classes.table}
                     aria-labelledby="tableTitle"
                     size={'small'}
                     aria-label="enhanced table"
                    >
                        <DefaultTableHeader 
                            headCells={headCells}
                            sortedColumn={sortInfo?.ColumnName}
                            sortedOrder={sortInfo?.Order}
                            onSort={handleSort}
                            classes={classes}
                        />
                        <TableBody>
                            {
                                searchResult.List.map((row, index) => {
                                    const isSelected = row.UserNo === selectedUserNo

                                    return (
                                        <StyledTableRow
                                            hover
                                            key={row.UserNo}
                                            onClick={e => handleRowClick(row.UserNo, e)}
                                            selected={isSelected}>

                                            <StyledTableCell align="center">
                                                {/* 編集画面へ遷移 */}
                                                <Tooltip placement="top" title="編集">
                                                    <IconButton 
                                                    aria-label="edit"
                                                    onClick={handleEdit(row.UserNo)}>
                                                        <EditIcon fontSize="small"/>
                                                    </IconButton>
                                                </Tooltip>
                                            </StyledTableCell>
                                            
                                            <StyledTableCell align="left" classes={classes.table_cell}>{row.UserName}</StyledTableCell>
                                            <StyledTableCell align="left">{row.LoginId}</StyledTableCell>
                                            <StyledTableCell align="left">{row.DivisionNameList?.join(",")}</StyledTableCell>
                                            <StyledTableCell align="left">{row.RoleName}</StyledTableCell>
                                            {/* <StyledTableCell align="center">
                                                <Tooltip title="削除">
                                                    <IconButton 
                                                    aria-label="delete"
                                                    onClick={handleDelete(row.UserNo)}>
                                                        <DeleteIcon fontSize="small"/>
                                                    </IconButton>
                                                </Tooltip>
                                            </StyledTableCell> */}
                                        </StyledTableRow>
                                    )
                                })
                            }
                            {
                                emptyRows > 0 && [...Array(emptyRows).keys()].map((row, index) => (
                                    <TableRow key={index}>
                                        <NoBorderCell colSpan={headCells.length} style={{textAlign: "center"}}>
                                            {searchResult.TotalCount === 0 && index === 0 ? '検索条件に合致する結果がありませんでした。' : ''}
                                        </NoBorderCell>
                                    </TableRow>
                                ))
                            }
                        </TableBody>
                    </Table>
                </TableContainer>
                <TablePagination 
                    rowsPerPageOptions={[5, 10, 25]}
                    component="div"
                    count={searchResult.TotalCount}
                    rowsPerPage={pagingInfo.RowCount}
                    page={pagingInfo.CurrentPage}
                    onChangePage={handleChangePage}
                    onChangeRowsPerPage={handleChangeRowsPerPage}
                />
           
        </Paper>
    )
}

export default SearchResult
import React, { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

import { makeStyles } from '@mui/styles';
import { Paper, Container, Box, Typography, Tooltip } from '@mui/material';
import { Table, TableBody, TableRow, TableContainer, TablePagination } from '@mui/material';

import IconButton from '@mui/material/IconButton';
import EditIcon from '@mui/icons-material/Edit';

//ルーティング用
import { BrowserRouter as Router, Switch, Route, Link, useParams, useRouteMatch, useHistory } from "react-router-dom";

import { yesNoDialogActions } from '~/global/stores/yesNoDialogSlice'
import { messageDialogActions } from '~/global/stores/messageDialogSlice'

import { ScrollableTableContainer, DefaultTableHeader, StyledTableRow, StyledTableCell, NoBorderCell } from '~/global/component/Table'
import { actions, searchList } from '../store/businessCodeMainteSlice'
import service from '../service/businessCodeMainteService'


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
        height: "58vh"
    },
    table: {
        minWidth: 1300,
        tableLayout: "fixed"
    }
}))


const headCells = [
    { id: 'edit', label: '編集', disablePadding: true, ignoreSort: true, style: { width: "70px" } },
    { id: 'AccountingCodeName', numeric: false, disablePadding: true, label: '会計名', style: { width: "70px" } },
    { id: 'DebitBusinessCode', numeric: false, disablePadding: true, label: '借方事業コード', style: { width: "150px" } },
    { id: 'DebitBusinessName', numeric: false, disablePadding: true, label: '借方事業名', style: { width: "auto" } },
    { id: 'DebitAccountingItemCode', numeric: false, disablePadding: true, label: '借方主科目', style: { width: "106px" } },
    { id: 'DebitAccountingAssistItemCode', numeric: false, disablePadding: true, label: '借方補助科目', style: { width: "116px" } },
    { id: 'DebitTaxCode', numeric: false, disablePadding: true, label: '借方税', style: { width: "80px" } },
    { id: 'CreditBusinessCode', numeric: false, disablePadding: true, label: '貸方事業コード', style: { width: "150px" } },
    { id: 'CreditAccountingItemCode', numeric: false, disablePadding: true, label: '貸方主科目', style: { width: "106px" } },
    { id: 'CreditAccountingAssistItemCode', numeric: false, disablePadding: true, label: '貸方補助科目', style: { width: "116px" } },
    { id: 'CreditTaxCode', numeric: false, disablePadding: true, label: '貸方税', style: { width: "80px" } }
]


//検索結果
//const SearchResult = ({searchedList, totalCount, onSearch, onDelete, onSelectedRow }) => {
const SearchResult = () => {
    const classes = useStyles()
    const dispatch = useDispatch()

    const sortInfoList = useSelector(state => state.businessCodeMainte.sortInfoList)
    const sortInfo = { ColumnName: sortInfoList.map(m => m.ColumnName).join(","), Order: sortInfoList.find(m => true)?.Order ?? "asc" }
    const pagingInfo = useSelector(state => state.businessCodeMainte.pagingInfo)
    const searchResult = useSelector(state => state.businessCodeMainte.searchResult)
    const selectedBusinessCodeNo = useSelector(state => state.businessCodeMainte.selectedBusinessCodeNo)

    //ルーティング用
    //親でマッチしたurlを取得。(ここの場合/view/mainte/businessCode)
    const { path, url } = useRouteMatch()
    const history = useHistory()

    //行選択時の処理
    const handleRowClick = (BusinessCodeNo, event) => {
        dispatch(actions.setSelectedBusinessCodeNo(BusinessCodeNo))

        //const selectedInfo = searchResult.List.find(each => each.BusinessCodeNo === BusinessCodeNo)
        //dispatch(actions.setUpdateState(selectedInfo)) //選択した行を登録フォームに反映。
    }

    //編集ボタン押下時の処理
    const handleEdit = BusinessCodeNo => (event) => {
        event.stopPropagation(); //イベントのバブリングをキャンセル。これをしないと行のonclickが呼ばれてしまう。

        dispatch(actions.setSelectedBusinessCodeNo(BusinessCodeNo))
        const selectedInfo = searchResult.List.find(each => each.BusinessCodeNo === BusinessCodeNo)
        dispatch(actions.setOriginalInfo(selectedInfo)) // 選択した行を登録フォームに反映。

        history.push(`${url}/edit/${BusinessCodeNo}`) // 編集画面へ遷移
    }

    //現在のページ変更時の処理
    const handleChangePage = async (event, newPage) => {
        const _pagingInfo = { ...pagingInfo, CurrentPage: newPage }
        dispatch(actions.setPagingInfo(_pagingInfo))

        dispatch(searchList({ pagingInfo: _pagingInfo }))
    }

    //１ページ毎の表示件数変更処理
    const handleChangeRowsPerPage = async (event) => {
        const _pagingInfo = { CurrentPage: 0, RowCount: parseInt(event.target.value, 10) }
        dispatch(actions.setPagingInfo(_pagingInfo))
        dispatch(searchList({ pagingInfo: _pagingInfo }))
    }

    //ソート時のイベント
    const handleSort = async (name, event) => {
        const isDesc = sortInfo?.ColumnName === name && sortInfo?.Order === 'desc';
        const order = isDesc ? 'asc' : 'desc';

        const _sortInfoList = name.split(',').map(m => { return { ColumnName: m, Order: order } })
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
                                const isSelected = row.BusinessCodeNo === selectedBusinessCodeNo

                                return (
                                    <StyledTableRow
                                        hover
                                        key={row.BusinessCodeNo}
                                        onClick={e => handleRowClick(row.BusinessCodeNo, e)}
                                        selected={isSelected}>

                                        <StyledTableCell align="center">
                                            {/* 編集画面へ遷移 */}
                                            <Tooltip title="編集">
                                                <IconButton
                                                    aria-label="edit"
                                                    onClick={handleEdit(row.BusinessCodeNo)}>
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                        </StyledTableCell>

                                        <StyledTableCell align="left" classes={classes.table_cell}>{row.AccountingCodeName}</StyledTableCell>
                                        <StyledTableCell align="left">{row.DebitBusinessCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.DebitBusinessName}</StyledTableCell>
                                        <StyledTableCell align="left">{row.DebitAccountingItemCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.DebitAccountingAssistItemCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.DebitTaxCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.CreditBusinessCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.CreditAccountingItemCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.CreditAccountingAssistItemCode}</StyledTableCell>
                                        <StyledTableCell align="left">{row.CreditTaxCode}</StyledTableCell>

                                    </StyledTableRow>
                                )
                            })
                        }
                        {/* {
                            emptyRows > 0 && [...Array(emptyRows).keys()].map((row, index) => (
                                <TableRow key={index}>
                                    <NoBorderCell colSpan={headCells.length} style={{ textAlign: "center" }}>
                                        {searchResult.TotalCount === 0 && index === 0 ? '検索条件に合致する結果がありませんでした。' : ''}
                                    </NoBorderCell>
                                </TableRow>
                            ))
                        } */}
                        {
                            (searchResult.TotalCount === 0) &&
                            <TableRow key={-1}>
                                <StyledTableCell colSpan={headCells.length} style={{ textAlign: "center" }}>
                                    {'検索条件に合致する結果がありませんでした。'}
                                </StyledTableCell>
                            </TableRow>
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
                onPageChange={handleChangePage}
                onRowsPerPageChange={handleChangeRowsPerPage}
            />
        </Paper>
    )
}

export default SearchResult
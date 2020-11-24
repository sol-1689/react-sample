import React from 'react'
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import TableSortLabel from '@material-ui/core/TableSortLabel';

import FormControlLabel from '@material-ui/core/FormControlLabel';

import {StyledTableCell} from './StyledTableCell'

//headCellsは以下のようなデータを想定。
// const headCells = [
//     {id: 'name', numeric: false, disablePadding: true, label: 'ユーザー名'},
//     {id: 'delete',  label: '削除', disablePadding: true, ignoreSort: true}
// ]

const defaultStyle = {
    wordWrap: "break-word",
    width: "auto"
  };

//検索結果テーブルのヘッダー
const DefaultTableHeader = ({ headCells, classes, sortedColumn, sortedOrder, onSort}) => {

    //ソート処理のイベントを作成する。（=>を2重にしてクロージャを作成している。）
    const createOnSortEvent = (name, _sortedOrder) => event => {
        onSort(name, event);
    }

    return (
        <TableHead>
            <TableRow>
                {
                    headCells.map(headCell => {

                        const style = {...defaultStyle, ...(headCell.style || {})}
                        
                        return (
                        <StyledTableCell
                            key={headCell.id}
                            align="center"
                            padding={headCell.disablePadding ? 'none' : 'default'}
                            style={style}
                            sortDirection={sortedColumn === headCell.id ? sortedOrder : false}
                        >
                            {headCell.ignoreSort === true ? 
                            (
                                //ソートしない列の場合
                                <label>{headCell.label}</label>
                            ):
                            (
                                <TableSortLabel
                                    active={sortedColumn === headCell.id}
                                    direction={sortedOrder}
                                    onClick={createOnSortEvent(headCell.id, sortedOrder)}
                                >
                                    {headCell.label}
                                    {/* {sortedColumn === headCell.id ? (
                                        <span>
                                            {sortedOrder === 'desc' ? 'sorted descending' : 'sorted ascending'}
                                        </span>
                                    ) : null} */}

                                </TableSortLabel>
                            )}
                            
                        </StyledTableCell>
                    )})
                }
            </TableRow>
        </TableHead>
    )
}

export default DefaultTableHeader
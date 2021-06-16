import React, { useRef, useLayoutEffect, useMemo } from 'react'
import { withStyles } from '@material-ui/core/styles';
import { TableContainer, TableHead, TableSortLabel, TableRow, TableCell } from '@material-ui/core';


//スクロール値を設定可能なTableContainer
export const ScrollableTableContainer = ({ scrollPositionY, list, children, ...otherProps }) => {

  const containerRef = useRef(null)

  //スクロール位置を変更する。
  const changeScrollY = (positionY) => {
    if (containerRef.current != null) {
      containerRef.current.scrollTop = positionY
    }
  }

  //スクロール位置の変更
  useLayoutEffect(() => {
    changeScrollY(scrollPositionY)

  })

  //スクロールされる度に再レンダリングされるのを防ぐためメモ化。
  //表示する一覧が変わった際にのみ再レンダリングするように。
  const Elements = useMemo(() => {
    return (
      <TableContainer ref={containerRef} {...otherProps}>
        {children}
      </TableContainer>
    )

  }, [list])

  return (
    <>
      {Elements}
    </>
  )
}


const defaultStyle = {
  wordWrap: "break-word",
  width: "auto"
};

//検索結果テーブルのヘッダー
export const DefaultTableHeader = ({ headCells, classes, sortedColumn, sortedOrder, onSort}) => {

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

//Tableのデフォルト行
export const StyledTableRow = withStyles(theme => ({
  //一行ごとに色分け
  root: {
    '&:nth-of-type(odd)': {
      backgroundColor: theme.palette.background.default,
    },
  },
}))(TableRow);


//Tableのデフォルトセル
export const StyledTableCell = withStyles(theme => ({
  head: {
    backgroundColor: theme.palette.grey[100],
    //color: theme.palette.grey[700],
    border: "1px solid",
    borderColor: theme.palette.grey[500]
  },
  body: {
    border: "1px solid",
    borderColor: theme.palette.grey[500],
    padding: "4px"
  },
}))(TableCell);

//ボーダーなしのセル。空白行などでの利用を想定。
export const NoBorderCell = withStyles(theme => ({
  head: {
    backgroundColor: theme.palette.grey[100],
    //color: theme.palette.grey[700],
    borderStyle: "none",
  },
  body: {
    borderStyle: "none",
  },
}))(TableCell);

  //export default StyledTableCell
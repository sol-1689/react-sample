import React from 'react'
import { withStyles }  from '@material-ui/core/styles';
import TableCell from '@material-ui/core/TableCell';

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
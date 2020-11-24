import React from 'react'
import { withStyles }  from '@material-ui/core/styles';
import TableRow from '@material-ui/core/TableRow';

//Tableのデフォルト行
const StyledTableRow = withStyles(theme => ({
    //一行ごとに色分け
    root: {
      '&:nth-of-type(odd)': {
        backgroundColor: theme.palette.background.default,
      },
    },
  }))(TableRow);

  export default StyledTableRow

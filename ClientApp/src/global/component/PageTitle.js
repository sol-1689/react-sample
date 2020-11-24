
import React, {useState, useEffect } from 'react'
import { Typography } from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles(theme => ({
    titleContainer:{
        paddingBottom: theme.spacing(1),
    },

    title:{
        textAlign:"left",
        paddingBottom: "1px",
        borderBottom: "1px solid #cccccc"
    },

    // layout: {
    //     width: 'auto',
    //     marginLeft: theme.spacing(2),
    //     marginRight: theme.spacing(2),
    //     [theme.breakpoints.up(600 + theme.spacing(2) * 2)]: {
    //       width: 600,
    //       marginLeft: 'auto',
    //       marginRight: 'auto',
    //     },
    // },
}))


//各画面のタイトルを表示する。
const PageTitle = ({title}) => {
    const classes = useStyles();

    return (
        <div className={classes.titleContainer}> 
            <Typography component="h1" variant="h6" className={classes.title}>
                {title}
            </Typography>
        </div>
    )
}

export default PageTitle
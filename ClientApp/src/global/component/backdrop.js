import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux'

import { makeStyles } from '@mui/styles';
import { Backdrop, CircularProgress, Typography } from '@mui/material';


const useStyles = makeStyles(theme => ({
    backdrop: {
        zIndex: theme.zIndex.modal + 1,
        color: '#fff',
    },

    circleContainer: {
        textAlign: "center"
    },

    messageContainer: {
        marginTop: theme.spacing(1),
        textAlign: "center"
    }
}));

//クルクル表示
const DefaultBackDrop = () => {
    const classes = useStyles()
    const open = useSelector(state => state.backdrop.open)
    const message = useSelector(state => state.backdrop.message)

    useEffect(() => {
        console.log("MyBackDropのuseEffect")
    })

    return (
        <div>
            <Backdrop
                className={classes.backdrop}
                open={open}
            >
                <div>
                    <div className={classes.circleContainer}>
                        <CircularProgress color="inherit"></CircularProgress>
                    </div>
                    {
                        message &&
                        (
                            <Typography className={classes.messageContainer}>
                                {message}
                            </Typography>
                        )
                    }
                </div>
            </Backdrop>
        </div>
    )
}

export default DefaultBackDrop
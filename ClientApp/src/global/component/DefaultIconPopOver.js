import React, { useState } from 'react'

import {IconButton, Popover} from '@material-ui/core';
import { withStyles, makeStyles } from '@material-ui/core/styles';


const useStyles = makeStyles(theme => ({
    popover: {
      pointerEvents: 'none',
    }
  }));

//登録時の情報表示エリア
export const DefaultIconClickPopOver = ({TextComponent, IconComponent, buttonProps, ...otherProps}) => {

    const [anchorEl, setAnchorEl] = React.useState(null);
    const open = Boolean(anchorEl);
    const id = open ? `popicon_${new Date().getTime()}` : undefined;

    const handleClick = event => {
        setAnchorEl(event.currentTarget);
      };
    
      const handleClose = () => {
        setAnchorEl(null);
      };
    

    return (
        <>
            <IconButton aria-describedby={id} size={"small"} {...(buttonProps || {})} onClick={handleClick}>
                {IconComponent}
            </IconButton>
            <Popover
                id={id}
                open={open}
                anchorEl={anchorEl}
                onClose={handleClose}
                {...otherProps}
            >
                {TextComponent}
            </Popover>
        </>
    )
}


//登録時の情報表示エリア
export const DefaultIconHoverPopOver = ({TextComponent, IconComponent, buttonProps, ...otherProps}) => {
    const classes = useStyles();

    const [anchorEl, setAnchorEl] = React.useState(null);
    const open = Boolean(anchorEl);
    const id = open ? `popicon_${new Date().getTime()}` : undefined;

    const handlePopoverOpen = event => {
        setAnchorEl(event.currentTarget);
      };
    
      const handleClose = () => {
        setAnchorEl(null);
      };
    

    return (
        <>
            <IconButton aria-owns={id} aria-haspopup="true" size={"small"} {...(buttonProps || {})} onMouseEnter={handlePopoverOpen} onMouseLeave={handleClose}>
                {IconComponent}
            </IconButton>
            <Popover
                id={id}
                className={classes.popover}
                open={open}
                anchorEl={anchorEl}
                onClose={handleClose}
                {...otherProps}
            >
                {TextComponent}
            </Popover>
        </>
    )
}


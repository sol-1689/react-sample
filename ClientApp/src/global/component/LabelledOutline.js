import React, { useState, useRef } from 'react';
import ReactDom from 'react-dom';

import InputLabel from '@material-ui/core/InputLabel';
import Box from '@material-ui/core/Box';
import NotchedOutline from "@material-ui/core/OutlinedInput/NotchedOutline";
import { makeStyles } from '@material-ui/core/styles';


const useStyles = makeStyles(theme => ({
    outlineContainer: {
        position: "relative"
    },
    root: {
        position: "relative"
    },
    content: {
        padding: theme.spacing(1)
    },
    inputLabel: {
        position: "absolute",
        left: 0,
        top: 0,
        // slight alteration to spec spacing to match visual spec result
        transform: "translate(0, 24px) scale(1)"
    }
}));

//枠線の左上部分にラベルが表示される外枠
const LabelledOutline = ({ id, label, children, className, ...otherProps }) => {
    const classes = useStyles()
    const [labelWidth, setLabelWidth] = useState(0);
    const labelRef = useRef(null);

    React.useEffect(() => {
        const labelNode = ReactDom.findDOMNode(labelRef.current);
        setLabelWidth(labelNode != null ? labelNode.offsetWidth : 0);
    }, [label]);

    return (
        <Box className={`${classes.outlineContainer} ${className}`} {...otherProps}>
            <InputLabel
                ref={labelRef}
                htmlFor={id}
                variant="outlined"
                className={classes.inputLabel}
                shrink
            >
                {label}
            </InputLabel>
            <div className={classes.root}>
                <div id={id} className={classes.content}>
                    {children}
                    <NotchedOutline notched labelWidth={labelWidth} />
                </div>
            </div>
        </Box>
    )
}

export default LabelledOutline
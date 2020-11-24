//共通メニュー

import React, { useState } from 'react'
import ReactDom from 'react-dom'
import * as constant from '~/util/constant'

import { NavLink } from 'react-router-dom'

import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import Menu from '@material-ui/core/Menu';
import MenuItem from '@material-ui/core/MenuItem';
import Link from '@material-ui/core/Link';
import Box from '@material-ui/core/Box';

import { makeStyles } from '@material-ui/core/styles';

import grey from '@material-ui/core/colors/grey';


const useStyles = makeStyles(theme => ({
    root: {
        flexGrow: 1
    },

    left: {
        flexGrow: 1
    },

    title: {
        display: 'none',
        [theme.breakpoints.up('sm')]: {
            display: 'inline-block',
            verticalAlign: "middle"
        }
    },

    appName: {
        fontSize: "1.3rem",
        textDecoration: 'none',
        '&:hover': {
            textDecoration: 'none',
        }
    },

    userName:{

    },

    menu_buttons: {
        marginLeft: theme.spacing(2)
    },
    menu_button: {
        color: grey[50],
        marginLeft: theme.spacing(2)
    },
    menu_link: {
        textDecoration: 'none',
        display: 'block',
        color: theme.palette.text.primary,
        '&:hover': {
            textDecoration: 'none',
        }
    }
}))

const MyMenu = () => {
    const classes = useStyles()

    //ログイン情報
    //AppParametersは、サーバー側の_SpaLayout.cshtmlにて設定している。
    const loginUserInfo = AppParameters.loginUserInfo



    //マスタメンテメニュー
    const mainteMenuList = [
        // トップは会議検索や役員検索などのダイアログ開発用　不要なのでコメントにしておく
        // { linkLabelText: "トップ", spaUrlBase: constant.mainte_view_base, toUrl: "/" },
        { linkLabelText: "ユーザー", spaUrlBase: constant.mainte_view_base, toUrl: "/user" },
        { linkLabelText: "事業コード", spaUrlBase: constant.mainte_view_base, toUrl: "/businessCode" },
    ]

    return (
        <div className={classes.root}>
            <AppBar position="static">
                <Toolbar>
                    <div className={classes.left}>
                        <Typography className={classes.title} noWrap>
                            <Link
                                component={Button}
                                ariant="body2"
                                color="inherit"
                                size="large"
                                className={classes.appName}
                                href={constant.top_page_url}
                                >
                                サンプルアプリ
                            </Link>
                        </Typography>


                        {
                            loginUserInfo.IsAdmin &&
                            (
                                <MenuGroup
                                    id="mainte-menu"
                                    buttonText="マスタ管理"
                                    linkInfoList={mainteMenuList}
                                />
                            )
                        }


                    </div>
                    {/* ユーザー名 */}
                    <Box pr={3} className={classes.userName}>{loginUserInfo.UserName}</Box>
                    <Button color="inherit" onClick={(e) => { location.href = constant.logout_page_url }}>ログアウト</Button>
                </Toolbar>
            </AppBar>
        </div>
    )
}



//各メニューグループ用のコンポーネント。
const MenuGroup = ({ id, buttonText, linkInfoList, ...otherProps }) => {
    const classes = useStyles()
    const [AnchorEl, setAnchorEl] = React.useState(null)

    const menuList = linkInfoList?.filter(i => !i.shouldHide)

    //表示対象が1件もなければボタン自体を非表示にする。
    if(!menuList || menuList.length < 1){
        return (<></>)
    }

    //メニュークリック時の処理
    const handleButtonClick = event => {
        setAnchorEl(event.currentTarget)
    }

    const handleMenuClose = () => {
        setAnchorEl(null)
    }

    return (
        <>
            <Button aria-controls={id} aria-haspopup="true" onClick={handleButtonClick} className={classes.menu_button}>
                {buttonText}
            </Button>
            <Menu
                id={id}
                anchorEl={AnchorEl}
                open={Boolean(AnchorEl)}
                onClose={handleMenuClose}
                {...otherProps}
            >
                {
                    menuList && menuList.map((info) => {
                        return (
                            <ForwardMenuLink key={info.toUrl} linkLabelText={info.linkLabelText} spaUrlBase={info.spaUrlBase} toUrl={info.toUrl} onClose={handleMenuClose} />
                        )
                    })
                }
            </Menu>
        </>
    )
}


//forwardRefを使わないと、「Function components cannot be given refs.」という警告が出る。
//https://material-ui.com/guides/composition/#caveat-with-inlining
//https://github.com/mui-org/material-ui/issues/15903
const ForwardMenuLink = React.forwardRef((props, ref) => (
    <MenuLink {...props} innerRef={ref} />
));

//各メニューリンク用のコンポーネント。
const MenuLink = ({ linkLabelText, spaUrlBase, toUrl, onClose }) => {
    const classes = useStyles()

    const isSpa = location.href.includes(spaUrlBase) //現在のurlがSPAの範囲内かをチェック。
    const fullToUrl = spaUrlBase + toUrl;

    return (
        isSpa ?
            (
                <MenuItem component={NavLink} to={fullToUrl} className={classes.menu_link} onClick={onClose}>{linkLabelText}</MenuItem>
            ) :
            (
                // SPA範囲内でなければ従来のページ遷移を行う。
                <MenuItem component={Link} href={fullToUrl} className={classes.menu_link} onClick={onClose}>{linkLabelText}</MenuItem>
            )
    )
}


export default MyMenu


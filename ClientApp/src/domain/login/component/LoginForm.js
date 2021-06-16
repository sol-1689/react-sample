import React, {useState, useEffect} from 'react'
import loginService from '../service/loginService'
import * as constant from '~/util/constant'

import CssBaseline from '@mui/material/CssBaseline';
import { Avatar, Button, TextField, FormControlLabel, Checkbox, Link, Grid, Box, Container, Typography } from '@mui/material';
import { makeStyles } from '@mui/styles';


const useStyles = makeStyles(theme => ({
    paper:{
        marginTop: theme.spacing(8),
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center'
    },
    avatar: {
        margin: theme.spacing(1),
        backgroundColor: theme.palette.secondary.main
    },
    form: {
        width: '100%',
        marginTop: theme.spacing(1)
    },
    submit:{
        margin: theme.spacing(3, 0, 2)
    },
    errorInfo:{
        color: theme.palette.error.main
    }
}))

const LoginFormContainer = () => {
    const classes = useStyles();

    useEffect(() => {
        document.title = "ログイン"
    }, [])

    {/* Containerのcomponentをmainにすると、IE11で幅指定が効かなくなる(IE11ではインライン要素として扱われるため) */}
    return (
        <Container component="div" maxWidth="xs">
            <div className={classes.paper}>
                <Avatar className={classes.avatar}>
                    {/* <LockOutlinedIcon/> */}
                </Avatar>
                <Typography component="h1" variant="h5">
                    サインイン
                </Typography>
                <LoginForm>
                    {/* 子コンポーネントは、親コンポーネント(ここではLoginForm)のprops.childrenに入る。 */}
                    
                </LoginForm>
            </div>
            <Box mt={8}>
               <SystemName/>
            </Box>
        </Container>
    )
}

//form部分
const LoginForm = (props) => {
     //空のオブジェクト渡すと、valueに設定する値がundefinedなせいで「uncontrolled input warning」的な警告が出る。
    const [loginInfo, setLoginInfo] = useState({Loginid:"user1", Password: "password"})
    const [errorMessages, setErrorMessages] = useState([])

    const classes = useStyles();

    const loginFunc = async () => {
        console.log("ログイン開始");
        //ログイン後に遷移する画面の指定があれば取得。
        const urlParams = new URLSearchParams(location.search)
        const returnUrl = urlParams.get("ReturnUrl")

        //ログイン処理
        const result = await loginService.login({...loginInfo, ReturnUrl: returnUrl});

        //エラーの場合
        if(!result.IsSuccess){
            setErrorMessages(result.ErrorMessages)
            return;
        }

        if(!!result.Result.RedirectUrl){
            location.href = result.Result.RedirectUrl;
        }else{
            location.href = constant.top_page_url;
        }
        console.log("ログイン終了");
    }

    const onSubmit = (e) => {
        e.preventDefault(); //デフォルトのsubmit処理を中止。
        loginFunc();
    }

    const FormStyle = {
        //backgroundColor: "white"
    }

    return (
        <form method="post" className={classes.form} onSubmit={onSubmit}>
            <TextField 
            name="loginid" 
            value={loginInfo.Loginid} 
            label="ユーザーID"
            //variant="outlined"
            margin="normal"
            size="medium"
            required
            fullWidth
            autoFocus
            onChange={(e) => setLoginInfo({...loginInfo, Loginid:e.target.value})}
            />

            <TextField 
            name="password" 
            value={loginInfo.Password} 
            label="パスワード"
            type="password"
            //variant="outlined"
            margin="normal"
            size="medium"
            required
            fullWidth
            autoComplete="on"
            onChange={(e) => setLoginInfo({...loginInfo, Password:e.target.value})}
            />
            
            {/* <FormControlLabel 
            　label="ログイン情報を維持する" 
            　control={<Checkbox value="remenber" color="primary"/>}
            /> */}

            <Button
             type="submit"
             fullWidth
             variant="contained"
             color="primary"
             className={classes.submit}>
                ログイン
            </Button>

            {/* エラー表示領域 */}
            <ErrorInfo errorMessages={errorMessages}/>

            {/* <Grid container>
                <Grid item xs>
                    <Link href="#" variant="body2">パスワードを忘れた場合</Link>
                </Grid>
                <Grid item>
                    <Link href="#" variant="body2">アカウントを作成</Link>
                </Grid>
            </Grid> */}
        </form>

    )
}



//エラー情報表示部分。
const ErrorInfo = ({errorMessages}) => {

    const classes = useStyles();

    if(!Array.isArray(errorMessages) || errorMessages.length === 0){
        return (<></>); //何か返さないとエラーになる。
    }

    return (
        <div className={classes.errorInfo}>
            {
                errorMessages.map((m, i) => (
                    <span key={i}>{m.Message}</span>
                    ))
            }
        </div>
    )
}

//システム情報
const SystemName = () => {
    return(
        <Typography variant="body2" color="textSecondary" align="center">
            {'サンプル'}
        </Typography>
    )
}


export default LoginFormContainer


import React, { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

//material-ui
import { makeStyles } from '@material-ui/core/styles';
import { Button, TextField, Typography, InputLabel, Checkbox, FormControl, FormControlLabel, FormHelperText, Select, Tooltip } from '@material-ui/core';
import { Paper, Container, Box, Grid } from '@material-ui/core';


//Validation用
import * as yup from 'yup';
import { validate, hasNoError } from '~/util/validateUtil'
import { ErrorMessage } from '~/global/component/ErrorMessage'

//ルーティング用
import { useParams, useHistory } from "react-router-dom";
import PageMoveHandler from '~/global/component/PageMoveHandler'

import { yesNoDialogActions } from '~/global/stores/yesNoDialogSlice'
import { messageDialogActions } from '~/global/stores/messageDialogSlice'
import { errorDialogActions } from '~/global/stores/errorDialogSlice'
import { actions, searchList, initialState } from '../store/userMainteSlice'
import service from '../service/userMainteService'
import { DefaultSelectList, DefaultMultiSelectList } from '~/global/component/selectList'
import PageTitle from '~/global/component/PageTitle'


const useStyles = makeStyles(theme => ({

    root: {
        width: '100%',
        height: '100%',

        //marginTop: theme.spacing(6),
        padding: theme.spacing(2, 1),

        // display: 'flex',
        // alignItems: 'center',
    },
    buttonContainer: {
        textAlign: "center"
    },
    backButton: {
        marginBottom: theme.spacing(2)
    },

    registerButton: {
        width: "50%"
    },
    formControl: {
        margin: theme.spacing(1),
        minWidth: 120,
    },

}))

// validationルールを定義。yupライブラリを利用。
// プロパティ名は対象要素のname属性と合わせる。
const validateScheme = (isUpdate) => {
    return yup.object().shape({
        UserName: yup.string()
            .required("ユーザー名は必須です。")
            .max(100, "ユーザー名は${max}文字以下で入力してください。"),
        LoginId: yup.string()
            .required("ログインIDは必須です。")
            .max(100, "ログインIDは${max}文字以下で入力してください。")
            .matches(/^[0-9a-zA-z\.]+$/, "半角英数字又は.（ピリオド）で入力して下さい。"),
        PasswordStr: isUpdate ? yup.string() : yup.string().required("パスワードは必須です。"),
        DivisionNoList: yup.array()
            .required("所属は必須です。"),
        Role: yup.string()
            .required("役割は必須です。")
    })
}

const RegisterForm = ({ type, userNo, baseUrl }) => {
    //reduxのdispatch。
    const dispatch = useDispatch()

    //urlからパラメータを取得。パラメータは親FormのRouteコンポーネントにて指定している。
    const history = useHistory();

    //編集か新規作成か。
    const isUpdate = type === 'edit' ? true : false;
    //validation用
    const scheme = validateScheme(isUpdate)

    //スタイル値。material-uiを利用。
    const classes = useStyles()

    const originalInfo = useSelector(state => state.userMainte.originalInfo)
    //editingTargetはこのコンポーネントでしか使わないため、reduxストアではなく普通のstateを使う。
    const [editingTarget, setEditingTarget] = useState({ ...originalInfo })
    const [errors, setErrors] = useState({}) //一度入力した後にリアルタイムチェックする為に利用。

    const messageType = isUpdate ? "更新" : "新規登録";
    const title = `ユーザーメンテ-${messageType}`

    useEffect(() => {
        //タイトルを設定。
        document.title = title

        let unmounted = false;

        //詳細情報を取得する。
        const setDetail = async (_userNo) => {
            if (!isUpdate) {
                dispatch(actions.setOriginalInfo(initialState.originalInfo)) //新規登録時は空オブジェクトを渡す
                return; //変更時のみサーバーから値を取り直す。
            }

            const result = await service.searchDetail(_userNo);

            if (result.IsSuccess) {
                const info = result.Result;
                //await処理中にコンポーネントがアンマウントされていた場合にストアを変更しないようにする。
                if (!unmounted) {
                    dispatch(actions.setOriginalInfo(info))
                    setEditingTarget({ ...info })
                }
            } else {
                dispatch(errorDialogActions.setDialogInfo({
                    open: true, title: "",
                    message: `データ取得処理でエラーが発生しました。`,
                }));
            }
        }

        setDetail(userNo);
        const cleanup = () => {
            unmounted = true;
        };
        return cleanup;
    }, [])

    //区分値
    const roleList = useSelector(state => state.userMainte.roleList)
    const divisionList = useSelector(state => state.userMainte.divisionList)


    //登録時のイベント
    const onSubmit = async (e) => {
        e.preventDefault(); //デフォルトのsubmit処理を中止。

        //入力値のvalidation
        //登録時は全入力欄をチェックする。
        const errors = validate({ ...editingTarget }, scheme);
        setErrors(errors)

        if (!hasNoError(errors)) {
            dispatch(errorDialogActions.setDialogInfo({
                open: true, title: "",
                message: `画面のメッセージに従い、入力値を修正してください。`,
            }));
            return;
        }

        dispatch(yesNoDialogActions.setDialogInfo({
            open: true,
            title: messageType,
            message: `${messageType}してよろしいですか？`,
            eventWhenOK: async () => {
                //新規登録もしくは更新
                if (isUpdate) {
                    await update()
                } else {
                    await insert()
                }
            }
        }));
    }

    //新規登録処理
    const insert = async () => {
        const result = await service.insert(editingTarget)

        if (result.IsSuccess) {
            dispatch(actions.setOriginalInfo({ ...editingTarget })) //登録後もそのまま編集中にする？
            dispatch(messageDialogActions.setDialogInfo({ open: true, title: "完了", message: `新規登録が完了しました。` }));
        }
    }

    //更新処理
    const update = async () => {
        const result = await service.update(editingTarget)

        if (result.IsSuccess) {
            dispatch(actions.setOriginalInfo({ ...editingTarget }))
            dispatch(messageDialogActions.setDialogInfo({ open: true, title: "完了", message: `更新が完了しました。` }));
        }
    }

    //キャンセル時のイベント
    const onCancel = (e) => {
        setEditingTarget({ ...originalInfo })
        setErrors({})
    }

    //テキスト変更時のイベント
    const handleTextChange = event => {
        //stateを更新し、UIを変更する。
        const valueObj = { [event.target.name]: event.target.value }
        setEditingTarget({ ...editingTarget, ...valueObj });

        //入力したフィールドのみエラーを表示する為の処理。
        //全てのフィールドに対するエラーが入っているので、入力したフィールドに対するエラー情報のみ抜き出し、stateに設定する。
        const eachErr = validate(valueObj, scheme);
        setErrors({ ...errors, [event.target.name]: eachErr[event.target.name] })
    };

    //複数選択リスト変更時のイベント
    const handleChangeMultiple = event => {
        const { name, value } = event.target;

        const valueObj = { [name]: value }
        setEditingTarget({ ...editingTarget, ...valueObj });

        //入力したフィールドのみエラーを表示する為の処理。
        const eachErr = validate(valueObj, scheme);
        setErrors({ ...errors, [name]: eachErr[name] })
    };

    //チェックボックス変更時のイベント
    const handleCheckboxChange = event => {
        const valueObj = { [event.target.name]: event.target.checked }
        setEditingTarget({ ...editingTarget, ...valueObj })

        const eachErr = validate(valueObj, scheme);
        setErrors({ ...errors, [event.target.name]: eachErr[event.target.name] })
    };

    //戻るボタン押下時の処理
    const onBack = () => {
        //history.replace(baseUrl) //こっちにすると必ず一覧に戻る（ブラウザに直接詳細画面のURLを入力して遷移してきた場合でも）
        history.goBack(); //ブラウザバックする。
    }

    //値が編集されているかをチェックする。
    const isModified = () => {
        const result = originalInfo.UserName != editingTarget.UserName ||
            originalInfo.LoginId != editingTarget.LoginId ||
            originalInfo.PasswordStr != editingTarget.PasswordStr ||
            originalInfo.Role != editingTarget.Role ||
            originalInfo.DivisionNoList?.toString() != editingTarget.DivisionNoList?.toString() ||
            originalInfo.IsDeleted != editingTarget.IsDeleted

        return result
    }

    const gridSpacing = 5;

    return (
        <Paper elevation={2} className={classes.root} >
            <Container maxWidth="lg">
                <PageTitle title={title} mb={2} />
                <Button
                    type="button"
                    size="small"
                    variant="contained"
                    color="secondary"
                    onClick={onBack}
                    className={classes.backButton}>
                    戻る
                </Button>


                <Grid container spacing={gridSpacing}>
                    <Grid item xs={12} md={6}>
                        {/* Controllerはvalidation用のコンポーネント */}
                        <TextField
                            name="UserName"
                            value={editingTarget.UserName}
                            label="ユーザー名"
                            placeholder="ユーザー名"
                            //disabled={inputDisabled}
                            onChange={handleTextChange}
                            //validation用定義
                            error={!!errors.UserName} //errors.UserNameがあればTrue。undefinedがないブラウザ用に一応2重否定にしている。
                            helperText={errors.UserName?.message}
                            //見た目定義
                            autoComplete='off'
                            //variant="outlined"
                            margin="normal"
                            fullWidth
                            // 以下を設定すると、ラベルが最初から上部に表示される
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={gridSpacing}>
                    <Grid item xs={12} md={6}>
                        <TextField
                            name="LoginId"
                            value={editingTarget.LoginId}
                            label="ログインID"
                            placeholder="ログインID"
                            onChange={handleTextChange}
                            //validation用定義
                            error={!!errors.LoginId} //undefinedがないブラウザ用に一応2重否定にしている。
                            helperText={errors.LoginId?.message}
                            //見た目定義
                            autoComplete='off'
                            //variant="outlined"
                            margin="normal"
                            fullWidth
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />

                    </Grid>

                    <Grid item xs={12} md={6}>
                        <TextField
                            name="PasswordStr"
                            value={editingTarget.PasswordStr || ""}
                            label="パスワード"
                            placeholder={isUpdate ? "パスワードを変更する場合のみ入力してください。" : "パスワード"}
                            type="password"
                            onChange={handleTextChange}
                            //validation用定義
                            error={!!errors.PasswordStr} //undefinedがないブラウザ用に一応2重否定にしている。
                            helperText={errors.PasswordStr?.message}
                            //見た目定義
                            autoComplete='off'
                            //variant="outlined"
                            margin="normal"
                            fullWidth
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                    </Grid>
                </Grid>

                <Grid container spacing={gridSpacing}>
                    <Grid item xs={12} md={6}>
                        <FormControl className={classes.formControl}>
                            <InputLabel shrink id="Lbl_DivisionNo">
                                所属科
                                </InputLabel>
                            <DefaultMultiSelectList
                                valueTextList={divisionList}
                                showNoSelect={true}
                                labelId="Lbl_DivisionNo"
                                name="DivisionNoList"
                                value={editingTarget.DivisionNoList}
                                onChange={handleChangeMultiple}
                                //validation用定義
                                error={!!errors.DivisionNoList} //undefinedがないブラウザ用に一応2重否定にしている。
                            />
                            {/* <FormHelperText>所属科を選択してください。</FormHelperText> */}
                            <ErrorMessage errors={errors} name="DivisionNoList" />
                        </FormControl>
                    </Grid>
                </Grid>

                <Grid container spacing={gridSpacing} alignItems={"center"}>
                    <Grid item xs={12} md={6}>
                        <FormControl className={classes.formControl}>
                            <InputLabel shrink id="Lbl_Role">
                                役割
                                </InputLabel>
                            <DefaultSelectList
                                valueTextList={roleList}
                                showNoSelect={true}
                                labelid="Lbl_Role"
                                name="Role"
                                value={editingTarget.Role}
                                onChange={handleTextChange}
                                //validation用定義
                                error={!!errors.Role} //undefinedがないブラウザ用に一応2重否定にしている。
                            />
                            <ErrorMessage errors={errors} name="Role" />
                        </FormControl>
                    </Grid>
                    <Grid item xs={12} md={6} style={{ textAlign: "right" }}>
                        <FormControlLabel
                            name="IsDeleted"
                            value="end"
                            control={<Checkbox color="primary" checked={editingTarget.IsDeleted} onChange={handleCheckboxChange} />}
                            label="削除"
                            labelPlacement="end"
                        />
                    </Grid>
                </Grid>

                <form method="post" onSubmit={onSubmit}>
                    <Grid container spacing={0}>
                        <Grid item xs={12} md={6} className={classes.buttonContainer}>
                            <Button
                                type="submit"
                                size="large"
                                variant="contained"
                                color="primary"
                                className={classes.registerButton}>
                                {messageType}
                            </Button>
                        </Grid>
                        <Grid item xs={12} md={6} className={classes.buttonContainer}>
                            <Button
                                type="button"
                                size="large"
                                variant="contained"
                                color="secondary"
                                onClick={onCancel}
                                className={classes.registerButton}>
                                キャンセル
                               </Button>
                        </Grid>
                    </Grid>
                </form>
            </Container>

            {/* 編集中の場合に確認メッセージを出す為のコンポーネント */}
            <PageMoveHandler isModified={isModified()} />
        </Paper>
    )
}


export default RegisterForm
import React, { useState, useEffect } from 'react'
import { useDispatch, useSelector } from 'react-redux'

//material-ui
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import { Paper } from '@material-ui/core';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Container from '@material-ui/core/Container';
import Grid from '@material-ui/core/Grid';
import FormHelperText from '@material-ui/core/FormHelperText';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import InputLabel from '@material-ui/core/InputLabel';
import Checkbox from '@material-ui/core/Checkbox';
import { DefaultSelectList } from '~/global/component/selectList'

//Validation用
import { validate, hasNoError } from '~/util/validateUtil'
import { ErrorMessage } from '~/global/component/ErrorMessage'
import * as yup from 'yup';

//ルーティング用
import { useParams, useHistory } from "react-router-dom";
import PageMoveHandler from '~/global/component/PageMoveHandler'

import { yesNoDialogActions } from '~/global/stores/yesNoDialogSlice'
import { messageDialogActions } from '~/global/stores/messageDialogSlice'
import { errorDialogActions } from '~/global/stores/errorDialogSlice'
import { actions, searchList } from '../store/businessCodeMainteSlice'
import service from '../service/businessCodeMainteService'


const useStyles = makeStyles(theme => ({
    root: {
        width: '100%',
        height: '100%',
        
        marginTop: theme.spacing(3),
        padding: theme.spacing(2, 0),
        // display: 'flex',
        // alignItems: 'center',
    },
    buttonContainer:{
        textAlign: "center"
    },
    backButton:{
        marginBottom: theme.spacing(2)
    },

    registerButtonsContainer:{
        marginTop: theme.spacing(2)
    },

    registerButton:{
        width: "50%"
    },
    formControl: {
        margin: theme.spacing(1),
        minWidth: 120,
    },
}))

//validationルールを定義。yupライブラリを利用。
const scheme = yup.object().shape({
    AccountingCodeNo: yup.string()
        .required("会計名は必須です。"),
    DebitBusinessName: yup.string()
        .required("借方事業名は必須です。")
        .max(100, "借方事業名は${max}文字以下で入力してください。"),
    DebitBusinessCode: yup.string()
        .required("借方事業コードは必須です。")
        .max(10, "借方事業コードは${max}文字以下で入力してください。")
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。"),    
    DebitAccountingItemCode: yup.string()
        .required("借方主科目は必須です。")
        .max(10, "借方主科目は${max}文字以下で入力してください。")
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。"),    
    DebitAccountingAssistItemCode: yup.string()
        .required("借方補助科目は必須です。")
        .max(10, "借方補助科目は${max}文字以下で入力してください。")
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。"),    
    DebitTaxCode: yup.string()
        .required("借方税は必須です。")
        .max(10, "借方税は${max}文字以下で入力してください。")
        .matches(/^[0-9a-zA-z]+$/, "半角英数字で入力して下さい。"),    
    CreditBusinessCode: yup.string()
        .required("貸方事業コードは必須です。")
        .max(10, "貸方事業コードは${max}文字以下で入力してください。")
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。"),    
    CreditAccountingItemCode: yup.string()
        .required("貸方主科目は必須です。")
        .max(10, "貸方主科目は${max}文字以下で入力してください。")
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。"),    
    CreditAccountingAssistItemCode: yup.string()
        .required("貸方補助科目は必須です。")
        .max(10, "貸方補助科目は${max}文字以下で入力してください。")
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。"),    
    CreditTaxCode: yup.string()
        .required("貸方税は必須です。")
        .max(10, "貸方税は${max}文字以下で入力してください。") 
        .matches(/^[0-9]+$/, "半角数字で入力して下さい。")   
})

const RegisterForm = () => {
    //reduxのdispatch。
    const dispatch = useDispatch()

    //スタイル値。material-uiを利用。
    const classes = useStyles()

    const originalInfo = useSelector(state => state.businessCodeMainte.originalInfo)
    //editingTargetはこのコンポーネントでしか使わないため、reduxストアではなく普通のstateを使う。
    const [editingTarget, setEditingTarget] = useState({ ...originalInfo })
    const [errors, setErrors] = useState({}) //一度入力した後にリアルタイムチェックする為に利用。

    //元のモデルから入力用のモデルを作成。
    useEffect(() => {
        console.log("RegisterFormのuseEffect")
        setEditingTarget({ ...originalInfo })
    }, [originalInfo])

    //urlからパラメータを取得。パラメータは親FormのRouteコンポーネントにて指定している。
    const { type, BusinessCodeNo } = useParams();
    const history = useHistory();

    //編集か新規作成か。
    const isUpdate = type === 'edit' ? true : false;
    const messageType = isUpdate ? "更新" : "新規登録";

    useEffect(() => {
        //タイトルを設定。
        document.title = `事業コードメンテ-${messageType}`
    }, [])

    //区分値
    const accountingCodeList = useSelector(state => state.businessCodeMainte.accountingCodeList)


    //登録時のイベント
    const onSubmit = async (e) => {
        e.preventDefault(); //デフォルトのsubmit処理を中止。

        //入力値のvalidation
        //登録時は全入力欄をチェックする。
        const errors = validate({...editingTarget}, scheme);
        setErrors(errors)
        if(!hasNoError(errors)){
            dispatch(errorDialogActions.setDialogInfo({open: true, title: "", 
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
            //登録後に再検索。ここではページングなどは変更しない。
            dispatch(searchList({}))
            dispatch(actions.setOriginalInfo({ ...editingTarget })) //登録後もそのまま編集中にする？
            dispatch(messageDialogActions.setDialogInfo({open: true, title: "完了", message: `新規登録が完了しました。`}));
        }
    }

    //更新処理
    const update = async () => {
        //TODO originalInfoとeditingTargetを比較して変更があれば登録。
        const result = await service.update(editingTarget)

        if (result.IsSuccess) {
            //登録後に再検索。ここではページングなどは変更しない。
            dispatch(searchList({}))
            dispatch(actions.setOriginalInfo({ ...editingTarget }))
            dispatch(messageDialogActions.setDialogInfo({open: true, title: "完了", message: `更新が完了しました。`}));
        }
    }

    //値が編集されているかをチェックする。
    const isModified = () => {
        const result = originalInfo.AccountingCodeNo != editingTarget.AccountingCodeNo ||
                        originalInfo.DebitBusinessName != editingTarget.DebitBusinessName ||
                        originalInfo.DebitBusinessCode != editingTarget.DebitBusinessCode ||
                        originalInfo.DebitAccountingItemCode != editingTarget.DebitAccountingItemCode ||
                        originalInfo.DebitAccountingAssistItemCode != editingTarget.DebitAccountingAssistItemCode ||
                        originalInfo.DebitTaxCode != editingTarget.DebitTaxCode ||
                        originalInfo.CreditBusinessCode != editingTarget.CreditBusinessCode ||
                        originalInfo.CreditAccountingItemCode != editingTarget.CreditAccountingItemCode ||
                        originalInfo.CreditAccountingAssistItemCode != editingTarget.CreditAccountingAssistItemCode ||
                        originalInfo.CreditTaxCode != editingTarget.CreditTaxCode;

        return result
    }

    //キャンセル時のイベント
    const onCancel = (e) => {
        setEditingTarget({ ...originalInfo })
        setErrors({})
    }

    //テキスト変更時のイベント
    const handleTextChange = event => {
        //stateを更新し、UIを変更する。
        const valueObj = {[event.target.name]: event.target.value}
        setEditingTarget({...editingTarget, ...valueObj});

        //入力したフィールドのみエラーを表示する為の処理。
        //全てのフィールドに対するエラーが入っているので、入力したフィールドに対するエラー情報のみ抜き出し、stateに設定する。
        const eachErr = validate(valueObj, scheme); 
        setErrors({...errors, [event.target.name]: eachErr[event.target.name]})
    };

    //チェックボックス変更時のイベント
    const handleCheckboxChange = event => {
        const valueObj = {[event.target.name]: event.target.checked}
        setEditingTarget({...editingTarget, ...valueObj})

        const eachErr = validate(valueObj, scheme); 
        setErrors({...errors, [event.target.name]: eachErr[event.target.name]})
    };

    //戻るボタン押下時の処理
    const onBack = () => {
        history.goBack(); //ブラウザバックする。
    }

    //編集中ではない場合は入力不可とする。
    //const inputDisabled = !isEditing

    const gridSpacing = 5

    return (
        <Paper elevation={2} className={classes.root} >
            <Container maxWidth="lg">
                <Button
                    type="button"
                    size="small"
                    variant="contained"
                    color="secondary"
                    onClick={onBack}
                    className={classes.backButton}>
                    戻る
                </Button>

                <form method="post" onSubmit={onSubmit}>
                    <Grid container>
                        <Grid container spacing={2}>
                            <Grid item xs={12} md={2}>
                                <FormControl className={classes.formControl}>
                                    <InputLabel shrink id="Lbl_AccountingCode">会計コード</InputLabel>
                                    <DefaultSelectList
                                        valueTextList={accountingCodeList}
                                        showNoSelect={true}
                                        labelid="Lbl_AccountingCode"
                                        name="AccountingCodeNo"
                                        value={editingTarget.AccountingCodeNo}
                                        onChange={handleTextChange}
                                        //validation用定義
                                        error={!!errors.AccountingCodeNo} //undefinedがないブラウザ用に一応2重否定にしている。
                                    />
                                    <ErrorMessage errors={errors} name="AccountingCodeNo" />
                                </FormControl>
                            </Grid>
                        </Grid>

                        <Grid container spacing={gridSpacing}>
                            <Grid item xs={12} md={6}>
                                <TextField
                                    name="DebitBusinessName"
                                    value={editingTarget.DebitBusinessName}
                                    label="借方事業名"
                                    placeholder="借方事業名"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.DebitBusinessName} //errors.UserNameがあればTrue。undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.DebitBusinessName?.message}
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
                                    name="DebitBusinessCode"
                                    value={editingTarget.DebitBusinessCode}
                                    label="借方事業コード"
                                    placeholder="借方事業コード"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.DebitBusinessCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.DebitBusinessCode?.message}
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
                                    name="DebitAccountingItemCode"
                                    value={editingTarget.DebitAccountingItemCode}
                                    label="借方主科目"
                                    placeholder="借方主科目"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.DebitAccountingItemCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.DebitAccountingItemCode?.message}
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
                                <TextField
                                    name="DebitAccountingAssistItemCode"
                                    value={editingTarget.DebitAccountingAssistItemCode}
                                    label="借方補助科目"
                                    placeholder="借方補助科目"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.DebitAccountingAssistItemCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.DebitAccountingAssistItemCode?.message}
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
                                    name="DebitTaxCode"
                                    value={editingTarget.DebitTaxCode}
                                    label="借方税"
                                    placeholder="借方税"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.DebitTaxCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.DebitTaxCode?.message}
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
                                <TextField
                                    name="CreditBusinessCode"
                                    value={editingTarget.CreditBusinessCode}
                                    label="貸方事業コード"
                                    placeholder="貸方事業コード"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.CreditBusinessCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.CreditBusinessCode ? errors.CreditBusinessCode.message : ""}
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
                                    name="CreditAccountingItemCode"
                                    value={editingTarget.CreditAccountingItemCode}
                                    label="貸方主科目"
                                    placeholder="貸方主科目"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.CreditAccountingItemCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.CreditAccountingItemCode?.message}
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
                                <TextField
                                    name="CreditAccountingAssistItemCode"
                                    value={editingTarget.CreditAccountingAssistItemCode}
                                    label="貸方補助科目"
                                    placeholder="貸方補助科目"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.CreditAccountingAssistItemCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.CreditAccountingAssistItemCode?.message}
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
                                    name="CreditTaxCode"
                                    value={editingTarget.CreditTaxCode}
                                    label="貸方税"
                                    placeholder="貸方税"
                                    onChange={handleTextChange}
                                    //validation用定義
                                    error={!!errors.CreditTaxCode} //undefinedがないブラウザ用に一応2重否定にしている。
                                    helperText={errors.CreditTaxCode?.message}
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



                        <Grid container spacing={gridSpacing} justify="flex-end">
                            <Grid item xs={12} md={6}>
                                <FormControlLabel
                                    name="IsPaid"
                                    control={<Checkbox color="primary" checked={editingTarget.IsPaid} onChange={handleCheckboxChange} />}
                                    label="支払対象"
                                    labelPlacement="end"
                                />
                            </Grid>

                            <Grid item xs={12} md={6}>
                                <FormControlLabel
                                    name="IsDeleted"
                                    control={<Checkbox color="primary" checked={editingTarget.IsDeleted} onChange={handleCheckboxChange} />}
                                    label="削除"
                                    labelPlacement="end"
                                />
                            </Grid>
                        </Grid>

                        <Grid container spacing={0} className={classes.registerButtonsContainer}>
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
                    </Grid>

                </form>
            </Container>

            {/* 編集中の場合に確認メッセージを出す為のコンポーネント */}
            <PageMoveHandler isModified={isModified()}/>
        </Paper>
    )
}

export default RegisterForm
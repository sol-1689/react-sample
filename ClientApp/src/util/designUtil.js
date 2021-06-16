import { createMuiTheme } from '@material-ui/core/styles';
import { jaJP } from '@material-ui/core/locale';

import { yellow, indigo, green } from '@material-ui/core/colors';

//material-uiのデフォルトテーマ
export const getDefaultTheme = () => {
    return createMuiTheme({
        //theme.spacing()の基準値。px。
        spacing: 8,

        //文字関連の設定
        //https://material-ui.com/customization/typography/
        typography: {
            fontSize: 12,
            //htmlFontSize: 10
        },

        //色
        palette:{
            //標準定義の上書き
            // primary:{
            //     light: blue["300"],
            //     main: blue["500"],
            //     dark: blue["700"],
            // },

            //独自定義も出来る。→増えすぎると分かりにくいので、別オブジェクトとして定義する。(ファイル下のEditedStyleなど)
            // editted:{
            //     main: yellow["500"],
            // }
        },

        //デフォルトのプロパティ。全体的に小さめにする。コンポーネントに対象プロパティが指定されている場合はそちらが優先される。
        props: {
            // MuiButton: {
            //   size: 'medium',
            //   //variant: "outlined", //外枠を表示
            // },

            // MuiIconButton: {
            //   size: 'small',
            // },

            // MuiFab: {
            //   size: 'small',
            // },
            // MuiTable: {
            //   size: 'small',
            // },

            //チェックボックスなど
            MuiCheckbox: {
              size: 'medium'
            },

            MuiSwitch: {
              size: 'medium'
            },

            MuiListItem: {
              dense: true,
            },
            
            MuiFormControl: {
              //margin: 'dense',
              margin: 'none',
            },

            MuiInputBase: {
              //margin: 'dense',
            },

            MuiTextField: {
              //margin: 'dense',
              variant: "outlined", //外枠を表示
              //size: "small"
            },

            MuiFilledInput: {
              //margin: 'dense',
            },

            //下記 MuiOutlinedInput、MuiFormHelperText、MuiInputLabelは、marginを合わせないと、テキスト内のラベルの位置が中心にならない。
            // MuiOutlinedInput: {
            //   margin: 'dense', //outlinedを指定したinputの、外枠と中のテキストとのmargin。
            // },

            // MuiFormHelperText: {
            //   margin: 'dense',
            // },
           
            // MuiInputLabel: {
            //  margin: 'dense',
            // },


            // MuiToolbar: {
            //   variant: 'dense',
            // },
        },

        //
        // mixins: {
        //     toolbar: {
        //         minHeight: 42
        //     }
        // },

        // classのstyleを上書き
        overrides: {
            MuiButton: {
                root: {
                     // ボタン内アルファベット文字を大文字変換しない
                    textTransform: 'none'
                }
            },
            MuiSwitch:{
                root: {

                }
            },

            //ラベル
            MuiFormControlLabel:{
              root:{
                marginLeft: "0px" //デフォルト-11pxになっているので左端がそろわない時がある。(特にチェックボックスを囲んだ際)
              }
            },
            //チェックボックスに付くクラス
            // PrivateSwitchBase:{
            //   root: {
            //     padding: "0 4px 0 4px",

            //   }
            // },
        }

    }, jaJP);
}

//material-uiのスタイルをマージする。
export function combineStyles(...styles) {
    return function CombineStyles(theme) {
      const outStyles = styles.map(arg => {
        // Apply the "theme" object for style functions.
        if (typeof arg === "function") {
          return arg(theme);
        }
        // Objects need no change.
        return arg;
      });
      return outStyles.reduce((acc, val) => Object.assign(acc, val));
    };
}
//export combineStyles;


//編集済みセル
export const EditedStyle = {
    color: yellow["50"],
}
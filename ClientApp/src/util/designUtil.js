import { createTheme } from '@mui/material/styles';
import { jaJP } from '@mui/material/locale';

import { yellow, indigo, green } from '@mui/material/colors';

//material-uiのデフォルトテーマ
export const getDefaultTheme = () => {
  return createTheme({
    // //theme.spacing()の基準値。px。
    spacing: 8,

    //文字関連の設定
    //https://material-ui.com/customization/typography/
    typography: {
      fontSize: 12,
      //htmlFontSize: 10
    },

    //余白を少なくし密度を高くする設定
    components: {
      MuiButton: {
        defaultProps: {
          size: 'small',
        },
      },
      MuiFilledInput: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiFormControl: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiFormHelperText: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiIconButton: {
        defaultProps: {
          size: 'small',
        },
      },
      MuiInputBase: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiInputLabel: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiListItem: {
        defaultProps: {
          dense: true,
        },
      },
      MuiOutlinedInput: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiFab: {
        defaultProps: {
          size: 'small',
        },
      },
      MuiTable: {
        defaultProps: {
          size: 'small',
        },
      },
      MuiTextField: {
        defaultProps: {
          margin: 'dense',
        },
      },
      MuiToolbar: {
        defaultProps: {
          variant: 'dense',
        },
      },
    },

    // //色
    // palette:{
    //     //標準定義の上書き
    //     // primary:{
    //     //     light: blue["300"],
    //     //     main: blue["500"],
    //     //     dark: blue["700"],
    //     // },

    //     //独自定義も出来る。→増えすぎると分かりにくいので、別オブジェクトとして定義する。(ファイル下のEditedStyleなど)
    //     // editted:{
    //     //     main: yellow["500"],
    //     // }
    // },


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
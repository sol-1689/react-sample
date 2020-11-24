//開発用・本番用設定の共通定義をまとめたファイル。

//プラグインを利用するために利用
const webpack = require('webpack');

//パス名指定時に利用
const path = require("path");

module.exports = {
    //context: path.resolve(__dirname, 'src'),

    //エントリーポイントの指定
    entry: { 
        login: './src/domain/login/app.js',
        mainte: './src/domain/mainte/app.js'
    },
    //output: {
    //    //出力先フォルダ
    //    path: path.resolve(__dirname, './public/js'),
    //    //出力ファイル名。[name]はentryのkey名。
    //    filename: '[name].bundle.js',

    //    //URLでアクセスする際のasset.
    //    //この例だと、http://server/public/js/bundle.js
    //    //publicPath: "public/"
    //},

    //配置場所によって相対パスの階層(../の数)が異なるのが煩雑だったので、~をルートパスとして絶対パスのように記述出来るようにする。
    //(例：../../global/component/menu.js → ~/global/component/menu.js)
    resolve: {
        alias: {
            '~': path.resolve(__dirname, 'src/'), // rootPath にはルートのパスが入る。
        }
    },
    
    module: {
        rules: [
            //babelローダーの指定
            {
                //処理対象ファイルを正規表現で指定。
                test: /\.js$/,
                //TODO フォルダ構成が決まったら処理対象ディレクトリを指定する。
                //include: path.resolve(__dirname, 'src/js'),
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        "presets": [
                            // targetを指定する必要はない。
                            // [
                            //     "@babel/preset-env",
                            //     {
                            //         //useBuiltIns: 'entry',
                            //         //useBuiltIns: 'usage',
                            //         //corejs: 3,
                            //         targets: {
                            //             //node: "current"
                            //             ie: "11", // IE11を指定。効かない？ので一旦コメントアウト。
                            //         },
                            //     }
                            // ], 
                            "@babel/preset-env", 
                            "@babel/preset-react", 
                        ],
                        //この記述は不要だった。
                        // "plugins": [
                        //     // ['@babel/plugin-transform-runtime', {
                        //     //     //regenerator: true,
                        //     //     corejs: 3
                        //     // }]
                        // ]
                    }
                }
            },
            // //css系のローダーを指定。
            // {
            //     //処理対象ファイルを正規表現で指定。
            //     test: /\.scss$/,
            //     //TODO フォルダ構成が決まったら処理対象ディレクトリを指定する。
            //     //include: path.resolve(__dirname, 'src/scss'),
            //     exclude: /node_modules/,
            //     //下から順に実行される
            //     use: [
            //         //HTMLに、変換したcssを参照する<style>タグを追加する。
            //         "style-loader",
            //         //CSSをモジュールに変換する。
            //         "css-loader",
            //         //Sassをcssにコンパイルする。
            //         "sass-loader"
            //     ]
            // },
            // //画像処理用ローダーを指定
            // {
            //     test: /\.(png|jpg|gif)$/i,
            //     //TODO フォルダ構成が決まったら処理対象ディレクトリを指定する。
            //     //include: path.resolve(__dirname, 'src/image'),
            //     exclude: /node_modules/,
            //     //画像をDataURLに変換するローダーを指定
            //     loader: "url-loader",
            //     options:
            //     {
            //         //画像サイズが8kb以上の場合はDataURLに変換しない
            //         limit: 8192,
            //         //[バンドル前のファイル名].[バンドル前のファイル拡張子]
            //         name: '[name].[ext]',
            //         //DataURLに変換しなかった画像の出力先。
            //         //出力先パスoutput.path(/public/js)からの相対パス。
            //         outputPath: '../images/',
            //         //出力されるファイル(cssなど）(この画像を参照するファイル？)からの画像の読み込み先。
            //         //出力されるファイルからの相対パス？
            //         publicPath: path => './images/' + path
            //     }
            // }
        ]
    },
    optimization: {
        //複数ファイルバンドル時に、共通の個所をまとめて１つのファイルにするための設定。
        splitChunks: {
            cacheGroups: {
                //vendor部分は任意の名前でよい。
                vendor: {
                    //node_modules配下のモジュールを共通化対象にする。
                    test: /node_modules/,

                    //filename.filenameの[name]にここの名前が入る。
                    name: "vendor",

                    //共通モジュールとしてバンドルする対象の設定
                    chunks: 'initial',

                    //デフォルトで有効ないくつかのsplitChunksの無効化。
                    enforce: true

                }

            }
        }

    }
};


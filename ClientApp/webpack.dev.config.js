//開発用設定ファイル。

//設定ファイルをマージするために利用
const { merge } = require('webpack-merge');

//共通設定を読み込む
const baseConfig = require("./webpack.base.config.js");

//パス名指定時に利用
const path = require("path");

//webpackに含まれるプラグインを利用する為に利用
const webpack = require("webpack");

//Reactのhotloader
const ReactRefreshWebpackPlugin = require('@pmmmwh/react-refresh-webpack-plugin');



module.exports = merge(baseConfig, {
    mode: 'development',
    output: {
        //出力先フォルダ
        path: path.resolve(__dirname, './public'),
        //出力ファイル名。[name]はentryのkey名。
        filename: '[name].bundle.js',

        //URLでアクセスする際のasset.
        //この例だと、http://server/public/js/bundle.js
        //publicPath: "public/"
    },
    //ソースマップの設定
    devtool: 'eval-cheap-module-source-map',

    plugins: [
        //環境ごとに異なる値はここで設定。
        //ソースからは「process.env.{設定値}」で呼び出せる。例)process.env.API_URL
        // new webpack.EnvironmentPlugin({
        //     //APIのリクエスト先。
        //     //API_URL: 'http://localhost:5001'　//不要だった。
        // }),

        //ReactのHMR用のプラグイン。
        new ReactRefreshWebpackPlugin(),
    ],

    devServer: {
        //公式パラメータ説明　https://webpack.js.org/configuration/dev-server/

        //localhost/{publicPath}でアクセスする。

        //ここで指定したディレクトリがpublicPath直下にバインドされる。
        //output.pathと合わせないと、writeToDisk: false(デフォルト)の際にファイルが作成されない？
        //このフォルダにゴミファイルがあると上手く動かない？
        //contentBase: path.resolve(__dirname, "./public"),
        static: {
            publicPath: "/",
            directory: path.resolve(__dirname, "./public"),
        },
        //compress: false,
        port: 8083,

        // この設定が無いと、コンテナ外からwebpack-devserverにアクセス出来ない。
        host: "0.0.0.0",

        //watchContentBase: true,
        hot: true, //HMRの有効

        //writeToDisk: true, //このオプションがないとDevServerのメモリ上にコンパイル後ファイルが保持される。その場合うまく更新されなかった？
        //https: true

        client: {
            webSocketURL: "auto://0.0.0.0:0/ws",
        },

        // client: {
        //     webSocketTransport: 'wss',
        // },
        // webSocketServer: 'wss',
    }
});


//本番用設定ファイル。

//設定ファイルをマージするために利用
const { merge } = require('webpack-merge');

//共通設定を読み込む
const baseConfig = require("./webpack.base.config.js");

//パス名指定時に利用
const path = require("path");

//optimization.minimizerを上書きするために必要なプラグイン。
const TerserPlugin = require('terser-webpack-plugin');

module.exports = merge(baseConfig, {
    mode: 'production',
    output: {
        //出力先フォルダ
        path: path.resolve(__dirname, '../ServerApp/WebApp/wwwroot/js'),
        //出力ファイル名。[name]はentryのkey名。
        filename: '[name].bundle.js',

        //URLでアクセスする際のasset.
        //この例だと、http://server/public/js/bundle.js
        //publicPath: "public/"
    },
    //productionモードで有効になるminimizer設定を上書きする。
    optimization: {
        minimizer: [
            new TerserPlugin({
                terserOptions: {
                    //consoleを削除する設定
                    compress: {drop_console:true}
                }
                ,extractComments: "all"
            })
        ]
    }
});


# ソースの開き方  
IDEはVisualStudioCode（以下vscode）を想定。  
vscodeは以下からダウンロード・インストールしておく。  
https://azure.microsoft.com/ja-jp/products/visual-studio-code/  

**vscodeでソースを開く手順**  
1.gitからソースをダウンロード(git cloneもしくはzipでのダウンロード)  
　zipの場合は解凍しておく。  
2.スタートメニューからVisualStudioCodeを開く  
3.画面上部の「File」→「Open Folder」でソースのダウンロード先フォルダを選択  
　（ソリューションファイル(ReactSample.sln)を直下に持つフォルダを選択する。）  

※vscodeのインストール時にCodeで開くを有効にしている場合は、以下の手順で開くことも可能。  
対象フォルダを右クリック→「Codeで開く」  


# 開発環境の設定
vscodeでソースファルダを開いた状態で、以下の作業を行う。

### コンテナを使用する場合
**①vscodeの設定**  
vscode左側メニューのExtensions(四角形が４つのアイコン)から、「Remote Development」をインストール

**②プロキシの設定**  
「.devcontainer/devcontainer.json」のプロキシ情報とコメントされている箇所を必要に応じて修正。(プロキシ無しの場合は空文字にする。)

**③開発コンテナの起動**  
右下の通知から「Reopen in Container」を選択。  
もしくは、左下の「><」マークから「Remote-Containers: Reopen in Container」を選択。

初回起動時は20分ほど時間が掛かります。(主にreact関連のライブラリのインストールで。)


### コンテナを使用しない場合

**①SDK等のインストール**  
開発端末に以下をインストール  

**node.js 推奨版**  
https://nodejs.org/ja/

**.NET SDK5.0**  
https://dotnet.microsoft.com/download
  
  
**②vscodeの設定**  
vscode左側メニューのExtensions(四角形が４つのアイコン)から、以下をインストール  
・C#  
・Git History  
  
    
**③Javascriptライブラリのインストール**  
vscode画面上部の「Terminal」→「Run Task..」→「win-npm-install」を選択
  
  
# DBコンテナの起動方法
「ServerApp/DB_Docker/start.bat」の実行で起動。  
同フォルダの「stop.bat」の実行で停止。  
(共にホスト側で実行してください。)  


# デバッグ方法

### C#側のデバッグ開始
vscode左側メニューのRun(虫のアイコン)をクリック → 上部のセレクトリストで「.NET Core Launch(Web)」が選択されていることを確認し、リスト左の実行アイコンをクリック。

### React側のデバッグ開始
**Linux（コンテナ内）の場合**  
vscode画面上部の「Terminal」→「Run Task..」→「linux-webpack-devserver」を選択

**Windowsの場合**  
vscode画面上部の「Terminal」→「Run Task..」→「win-webpack-devserver」を選択

両方を起動後、ホスト側のブラウザで「http://localhost:5001」にアクセスするとログインページが表示されます。  
ID・パスワードは以下になります。  
user1  
password  

※起動ポートは、.vscodeフォルダ内のlaunch.jsonファイルに記述されています。  




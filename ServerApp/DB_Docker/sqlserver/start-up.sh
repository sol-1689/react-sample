#!/bin/bash


# wait-for-it.sh終了直後に実行すると、まだSQLServerが起動しきってないのか？、照合順序がうんぬんのエラーになる場合がある。念のため少しsleepしておく。
sleep 5

# DB・テーブル作成と共通の初期データ登録を行う。
importData()
{
	files=("/init-data/create_db.sql" "/init-data/create_table.sql" "/init-data/create_view.sql" "/init-data/create_trigger.sql" "/init-data/create_type.sql" "/init-data/create_common_data.sql")

	for filepath in "${files[@]}"
	do
		echo "import: " "$filepath"
		# 既にSQLServerは起動している想定。(このshの呼び出し元でwait-for-it.shを利用してSQLServerの起動を待機している。)
		/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -i "$filepath"
	done

	# 各自の環境用のデータ登録を行う。
	for eachFile in `\find /init-data/additional -type f -name "*.sql"`
	do
	    echo "import: " "$eachFile"
		# 最後の -I パラメータによってQUOTED_IDENTIFIERがONになる。ONにしておかないとIndexの作成コマンドが失敗する？
		# https://stackoverflow.com/questions/51200915/sql-server-in-docker-create-index-failed-because-the-following-set-options-have
		/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -i "$eachFile" -I
	done
}

# 関数渡せずだめだった
# /usr/src/wait-for-it.sh localhost:1433 -t 30 -- importData

importData

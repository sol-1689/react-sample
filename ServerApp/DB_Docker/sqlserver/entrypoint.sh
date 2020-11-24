#!/bin/bash
# あえて&で連結してstart-up.shを待たない。start-up.sh内でsleepしてるうちにsqlservrが呼び出される想定。 
/usr/src/wait-for-it.sh localhost:1433 -t 180 -- /usr/src/start-up.sh & echo 'start SQLServer' & /opt/mssql/bin/sqlservr

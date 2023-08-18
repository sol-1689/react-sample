FROM mcr.microsoft.com/mssql/server:2019-latest

COPY ./start-up.sh /usr/src
COPY ./entrypoint.sh /usr/src
COPY ./wait-for-it.sh /usr/src

ENV TZ="Asia/Tokyo"

# RUN chmod +x /usr/src/start-up.sh && chmod +x /usr/src/entrypoint.sh && chmod +x /usr/src/wait-for-it.sh

# CMD ["/bin/bash", "-c", "/opt/mssql/bin/sqlservr & /usr/src/start-up.sh & /opt/mssql/bin/sqlservr"]
# CMD /bin/bash /usr/src/entrypoint.sh

CMD ["/bin/bash", "-c", "/usr/src/entrypoint.sh"]

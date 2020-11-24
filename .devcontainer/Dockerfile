
ARG DOTNETCORE_VESRION=3.1
FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNETCORE_VESRION}

# webpack-devsererを実行するポートをリッスンしておく。
EXPOSE 8083


# プロキシ設定。devcontainer.json内で指定した値がパラメータとして設定される。
ENV http_proxy=$http_proxy
ENV https_proxy=$https_proxy
ENV NO_PROXY=$NO_PROXY


# Timezoneを変更(Debianの場合)
RUN rm /etc/localtime && echo Asia/Tokyo > /etc/timezone && dpkg-reconfigure -f noninteractive tzdata

# localeの変更
RUN apt update && apt-get install -y locales && locale-gen ja_JP.UTF-8
ENV LANG ja_JP.UTF-8
ENV LC_CTYPE ja_JP.UTF-8
RUN localedef -f UTF-8 -i ja_JP ja_JP.utf8

# rootではない 'vscode' ユーザーを追加。
# ホストがLinuxの場合は、ホスト側のUID/GIDと一致していないとbind mounts時に権限エラーが出る場合はある。 
# USER_GIDをデフォルト1000にしているが、上記を踏まえ適宜変更する事。(ホストがWindowsならこのままで良さそう。)
# 詳細：https://aka.ms/vscode-remote/containers/non-root-user
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

# node.jsをインストールする場合
ARG INSTALL_NODE="true"

# Puppeteer-Sharpを利用する場合
ARG USE_CHROMIUM="false"

# node.jsのバージョンを指定
ARG NODE_VERSION="12.13.0"
# 常に最新バージョンを取りたい場合は以下の記述にする。
# ARG NODE_VERSION="lts/*"

ENV NVM_DIR=/home/vscode/.nvm

# 非対話型に切り替えることで警告を回避。
ENV DEBIAN_FRONTEND=noninteractive

# 必要なパッケージ類をインストール。
RUN apt-get update \
    && apt-get -y install --no-install-recommends apt-utils dialog 2>&1 \
    #
    # git、プロセスツール、lsb-release（CLIのインストール手順に共通）がインストールされていることを確認します。
    && apt-get -y install git iproute2 procps apt-transport-https gnupg2 curl lsb-release \
    #
    # 必要に応じて使用する非rootユーザーを作成します。 - see https://aka.ms/vscode-remote/containers/non-root-user.
    && groupadd --gid $USER_GID $USERNAME \
    && useradd -s /bin/bash --uid $USER_UID --gid $USER_GID -m $USERNAME \
    # [Optional] 非rootユーザーのsudoサポートを追加します。
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME\
    && chmod 0440 /etc/sudoers.d/$USERNAME \
    #
    # [Optional] node.jsのインストール。
    && if [ "$INSTALL_NODE" = "true" ]; then \
        # nvmとNode.jsのインストール。
        mkdir ${NVM_DIR} \
        && curl -so- https://raw.githubusercontent.com/creationix/nvm/v0.35.1/install.sh | bash 2>&1 \
        && chown -R vscode:vscode ${NVM_DIR} \
        && /bin/bash -c "source $NVM_DIR/nvm.sh \
            && nvm install ${NODE_VERSION} \
            && nvm alias default ${NODE_VERSION}" 2>&1 \
        && INIT_STRING='[ -s "$NVM_DIR/nvm.sh" ] && \\. "$NVM_DIR/nvm.sh"  && [ -s "$NVM_DIR/bash_completion" ] && \\. "$NVM_DIR/bash_completion"' \
        && echo $INIT_STRING >> /home/vscode/.bashrc \
        && echo $INIT_STRING >> /home/vscode/.zshrc \
        && echo $INIT_STRING >> /root/.zshrc \
        # yarnのインストール。
        && curl -sS https://dl.yarnpkg.com/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/pubkey.gpg | apt-key add - 2>/dev/null \
        && echo "deb https://dl.yarnpkg.com/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/ stable main" | tee /etc/apt/sources.list.d/yarn.list \
        && apt-get update \
        && apt-get -y install --no-install-recommends yarn; \
    fi \
    # [Optional] Puppeteer-Sharp関連のインストール。
    # 参考:http://www.hardkoded.com/blog/puppeteer-sharp-docker
    && if [ "$USE_CHROMIUM" = "true" ]; then \
        apt-get -f install && apt-get -y install wget gnupg2 apt-utils \
        && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
        && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
        && apt-get update \
        && apt-get install -y google-chrome-unstable fonts-ipafont-gothic \
        --no-install-recommends \
        && rm -rf /var/lib/apt/lists/* \
        && groupadd -r pptruser && useradd -r -g pptruser -G audio,video pptruser \
        && mkdir -p /home/pptruser/Downloads \
        && chown -R pptruser:pptruser /home/pptruser; \
    fi \
    # 不要なファイルを削除。
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Puppeteer-Sharp用。使わないなら無視されるだけ。
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-unstable"

# PATHにnode.jsのbinを追加しないと、devcontainer.jsonのpostCreateCommandでnpm: command not foundとなるので注意。
ENV NODE_PATH $NVM_DIR/v$NODE_VERSION/lib/node_modules
ENV PATH "${NVM_DIR}/versions/node/v${NODE_VERSION}/bin:$PATH"

ENV DEBIAN_FRONTEND=
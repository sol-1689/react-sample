import React from 'react';


//csrf対策のトークンを含んだformを作成する。
const CsrfTokenForm = ({children, ...otherProps}) => {
    //AppParametersは、サーバー側の_SpaLayout.cshtmlにて設定している。
    const csrfToken = AppParameters.XsrfToken

    return (
        <form {...otherProps}>
            {/* nameはこれにしないとasp coreフレームワークでチェックしてくれない。 */}
            <input type="hidden" name="__RequestVerificationToken" value={csrfToken}></input>
            {children}
        </form>
    )
}

export default CsrfTokenForm
import React, {useEffect} from 'react'
//ルーティング用
import {  Prompt } from "react-router-dom";

//ページ遷移時の確認メッセージを表示する。
const PageMoveHandler = ({isModified}) => {
    useEffect(() => {
        if (isModified) {
            window.onbeforeunload = () => true
        } else {
            window.onbeforeunload = undefined
        }

        //useEffect内でreturnした関数は、クリーンアップ関数となる。
        //クリーンアップ関数は、次にこのuseEffectが呼ばれたタイミングと、コンポーネントがアンマウントされる際に実行される。
        return () => {
            //ページから離脱したのにonbeforeunloadイベントが残り続けるのを防止するため、undefinedを設定する。
            window.onbeforeunload = undefined
        }
    })

    return(
        <Prompt
         when={isModified}
         message={"編集中のデータが存在します。ページを移動してよろしいですか？"}
        />
    )
}

export default PageMoveHandler
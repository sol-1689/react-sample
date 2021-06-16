//定数系を保持する。

//コンテキストパス。サーバー側の対応するappsettings.jsonで定義している。
//無ければ空文字。ある場合、先頭にスラッシュが付いている想定。
//AppParametersは、サーバー側の_SpaLayout.cshtmlにて設定している。
export const contextPath = AppParameters.ContextPath;

//トップページへの遷移用
export const top_page_url = `${contextPath}/Top/Index`

//SPA遷移する画面の基本URL
export const view_base = `${contextPath}/view`

//ログアウト用
export const logout_page_url =`${contextPath}/view/Login/Logout`

//メンテ画面用への遷移用
export const mainte_view_base = `${contextPath}/view/mainte`


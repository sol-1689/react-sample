import serviceUtil from '~/util/serviceUtil'

const urlBase = "/api/login"

//ログイン処理を行う。
const login = async (loginInfo) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        const result = await serviceUtil.postAndResult(`${urlBase}`, loginInfo);
        return result;
    })
}

export default { login }
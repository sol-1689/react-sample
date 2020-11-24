import serviceUtil from '~/util/serviceUtil'

const urlBase = "/api/kubun"


//区分一覧取得処理を行う。
const getRoleList = async () => {
    const result = await serviceUtil.postAndResult(`${urlBase}/list`, {CategoryId: "Role"});
    return result;
}

//課の一覧取得処理を行う。
const getDivisionList = async () => {
    const result = await serviceUtil.postAndResult(`${urlBase}/division`);
    return result;
}

//会計コード一覧取得処理を行う。
const getAccountingCodeList = async () => {
    const result = await serviceUtil.postAndResult(`${urlBase}/list`, {CategoryId: "AcountingCode"});
    result.Result.forEach(e => {
        e.Text = e.Remarks;
    });
    return result;
}

//事業コード一覧取得処理を行う。
const getBusinessCodeList = async () => {
    const result = await serviceUtil.postAndResult(`${urlBase}/businessCode`);
    return result;
}



export default {
    getRoleList, getDivisionList, getAccountingCodeList, getBusinessCodeList
}


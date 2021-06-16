import serviceUtil from '~/util/serviceUtil'
import { actions } from '../store/userMainteSlice' 
import store from '~/global/stores/mainStore'

const urlBase = "/api/mainte/user"

//検索処理を行う。
// sliceに移動
const search = async (searchInfo, SortParams, PagingParam) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        const result = await serviceUtil.postAndResult(`${urlBase}/search`, {...searchInfo, SortParams: SortParams, PagingParam:PagingParam});
        return result;
    })
}

//詳細情報を検索する。
const searchDetail = async (userNo) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        const result = await serviceUtil.postAndResult(`${urlBase}/detail`, { SearchUserNo: userNo });
        return result;
    })
}

const insert = async (insertInfo) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        return await serviceUtil.postAndResult(`${urlBase}/register`, insertInfo);
    })
}

const update = async (updateInfo) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        return await serviceUtil.postAndResult(`${urlBase}/update`, updateInfo);
    })
}

const deleteFunc = async (deleteInfo) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        return await serviceUtil.postAndResult(`${urlBase}/delete`, deleteInfo);
    })
}


export default { search, searchDetail, insert, update, deleteFunc }
import serviceUtil from '~/util/serviceUtil'
import { actions } from '../store/businessCodeMainteSlice' 
import store from '~/global/stores/mainStore'

const urlBase = "/api/mainte/businessCode"

//検索処理を行う。
// sliceに移動
const search = async (searchInfo, SortParams, PagingParam) => {
    return await serviceUtil.actionWithBackdrop(async () => {
        const result = await serviceUtil.postAndResult(`${urlBase}/search`, {...searchInfo, SortParams: SortParams, PagingParam:PagingParam});
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

export default { search, insert, update }
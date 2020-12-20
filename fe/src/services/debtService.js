import { url as base } from './url'

export const debtsInfo = (token, group_id) =>{
    const url = base + '/Debts/groupinfo?token=' + token + '&id=' + group_id

    return fetch(url, { method: 'get' }).then(res => res.json())
}
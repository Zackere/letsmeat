import { url as base } from './url'

export const debtsInfo = (token, group_id) => {
  const url = base + '/Debts/groupinfo?token=' + token + '&id=' + group_id

  return fetch(url, { method: 'get' }).then(res => res.json())
}

export const addDebt = (token, body) => {
  const url = base + '/Debts/add?token=' + token

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const getPendingDebts = token => {
  const url = base + '/Debts/pending?token=' + token

  return fetch(url, { method: 'get' }).then(res => res.json())
}

export const approveDebt = (token, debt_id) => {
  const url = base + '/Debts/approve?token=' + token
  const body = { debt_id }

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const rejectDebt = (token, debt_id) => {
  const url = base + '/Debts/reject?token=' + token
  const body = { debt_id }

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

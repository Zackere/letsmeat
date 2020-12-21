import { url as base } from './url'

export const getUserToken = token => {
  const url = base + '/Login/google?googleTokenId=' + token

  return fetch(url, { method: 'post' }).then(res => res.text())
}

export const searchUsers = (token, name) => {
  const url = base + '/Users/search?token=' + token + '&name=' + name

  return fetch(url, { method: 'get' }).then(res => res.json())
}

export const getUsers = (token, ids) => {
  const url = base + '/Users/info?token=' + token
  const body = { ids }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

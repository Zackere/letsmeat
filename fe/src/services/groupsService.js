import { url as base } from './constants'

export const getGroups = token => {
  const url = base + '/Users/info?token=' + token

  return fetch(url, { method: 'get' })
    .then(res => res.json())
    .then(user => user.groups)
}

export const getGroup = (token, id) => {
  const url = base + '/Groups/info?token=' + token + '&id=' + id

  return fetch(url, { method: 'get' }).then(res => res.json())
}

export const addGroup = (token, name) => {
  const url = base + '/Groups/create?token=' + token
  const body = { name }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const joinGroup = (token, id) => {
  const url = base + '/Groups/join?token=' + token
  const body = { id }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const leaveGroup = (token, id) => {
  const url = base + '/Groups/leave?token=' + token
  const body = { id }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const deleteGroup = (token, id) => {
  const url = base + '/Groups/delete?token=' + token
  const body = { id }

  return fetch(url, {
    method: 'delete',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

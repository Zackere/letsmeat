import { url as base } from './url'

export const addEvent = (name, deadline, group_id, token) => {
  const url = base + '/Events/create?token=' + token

  const body = {
    group_id,
    name,
    deadline,
  }
  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const deleteEvent = (token, id) => {
  const url = base + '/Events/delete?token=' + token
  const body = { id }

  return fetch(url, {
    method: 'delete',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const getEvent = (token,id) => {
  console.log(id)
  const url = base + '/Events/info?token=' + token + '&id=' + id

  return fetch(url, { method: 'get' }).then(res => res.json())
}


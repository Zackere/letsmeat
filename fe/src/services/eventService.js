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

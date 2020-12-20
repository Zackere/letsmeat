import { url as base } from './url'

export const sendInvitation = (token, groupId, userId) => {
  const url = base + '/Invitations/send?token=' + token
  const body = {
    to_id: userId,
    group_id: groupId,
  }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const getInvitations = token => {
  const url = base + '/Invitations/get?token=' + token

  return fetch(url, { method: 'get' }).then(res => res.json())
}
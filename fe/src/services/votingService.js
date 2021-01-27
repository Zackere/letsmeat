import { url as base } from './constants'

export const getDates = (token, group_id) => {
  const url = base + '/Debts/groupinfo?token=' + token + '&id=' + group_id

  return fetch(url, { method: 'get' }).then(res => res.json())
}

export const castVote = (token, times, locations, event_id) => {
  const url = base + '/Votes/cast?token=' + token
  const body = { vote_information: { times, locations }, event_id }

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const getVote = (token, event_id) => {
  const url = base + '/Votes/get?token=' + token + '&event_id=' + event_id

  return fetch(url, { method: 'GET' }).then(res => res.json())
}

export const getResult = (token, event_id) => {
  const url = base + '/Votes/result?token=' + token + '&event_id=' + event_id

  return fetch(url, { method: 'GET' }).then(res => res.json())
}

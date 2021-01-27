import { url as base } from './constants'

export const getPreferences = token => {
  const url = base + '/Users/info?token=' + token

  return fetch(url, { method: 'get' })
    .then(res => res.json())
    .then(user => user.prefs)
}

export const setPreferences = (token, prefs) => {
  const url = base + '/Users/update_prefs?token=' + token

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(prefs),
  })
}

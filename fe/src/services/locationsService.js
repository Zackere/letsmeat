import { url as base } from './url'

export const getLocations = (token, body) => {
  const url = base + '/Locations/info?token=' + token

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

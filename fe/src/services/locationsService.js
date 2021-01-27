import { url as base } from './constants'

export const getLocations = (token, body) => {
  const url = base + '/Locations/info?token=' + token

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const createGoogleLocation = (token, sessiontoken, place_id) => {
  const url = base + '/Locations/create_from_gmaps?token=' + token
  const body = { place_id, sessiontoken }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const createCustomLocation = (token, name, address, group_id) => {
  const url = base + '/Locations/create_custom?token=' + token
  const body = { name, address, group_id }

  return fetch(url, {
    method: 'post',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const searchLocation = (token, group_id, query_string, sessiontoken) => {
  const url =
    base +
    '/Locations/search?token=' +
    token +
    '&group_id=' +
    group_id +
    '&query_string=' +
    query_string +
    '&sessiontoken=' +
    sessiontoken

  return fetch(url, { method: 'get' }).then(res => res.json())
}

export const rateLocation = (token, body) => {
  const url = base + '/Locations/rate?token=' + token

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

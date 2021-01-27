import { url as base } from './constants'

export const addEvent = (name, deadline, group_id, token) => {
  const url = base + '/Events/create?token=' + token

  const body = {
    group_id,
    name,
    deadline,
  }

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const getImageDebts = (token, event_id) => {
  const url =
    base + '/Events/image_debts?token=' + token + '&event_id=' + event_id

  return fetch(url, { method: 'get' }).then(res => res.json())
}
export const updateEvent = (token, event) => {
  const url = base + '/Events/update?token=' + token

  return fetch(url, {
    method: 'PATCH',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(adjustFieldsNames(event)),
  })
}

export const deleteEvent = (token, id) => {
  const url = base + '/Events/delete?token=' + token
  const body = { id }

  return fetch(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

export const getEvent = (token, id) => {
  const url = base + '/Events/info?token=' + token + '&id=' + id

  return fetch(url, { method: 'get' }).then(res => res.json())
}

const adjustFieldsNames = event => ({
  id: event.id,
  custom_locations_ids: event.candidate_custom_locations,
  google_maps_locations_ids: event.candidate_google_maps_locations,
  candidate_times: event.candidate_times,
})

import { url as base } from './constants'

export const processReceipt = (token, event_id, file) => {
  const url = base + '/Images/upload?token=' + token + '&event_id=' + event_id
  const formData = new FormData()

  formData.append('file', file)

  return fetch(url, {
    method: 'POST',
    body: formData,
  })
}

export const getImages = (token, image_ids) => {
  const url = base + '/Images/info?token=' + token

  const body = { image_ids }

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  }).then(res => res.json())
}

export const updateImageDebt = (token, debt) => {
  const url = base + '/Images/update_image_debt?token=' + token

  return fetch(url, {
    method: 'PATCH',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(debt),
  })
}

export const addImageDebt = (token, debt) => {
  const url = base + '/Images/create_image_debt?token=' + token

  return fetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(debt),
  }).then(res => res.json())
}

export const deleteImageDebt = (token, id) => {
  const url = base + '/Images/delete_image_debt?token=' + token

  const body = { id }

  return fetch(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
}

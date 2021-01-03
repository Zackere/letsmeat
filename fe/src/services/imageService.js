import { url as base } from './url'

export const processReceipt = (token, event_id, file) => {
  const url = base + '/Images/upload?token=' + token + '&event_id=' + event_id
  const formData = new FormData()

  formData.append('file', file)

  return fetch(url, {
    method: 'POST',
    body: formData,
  })
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

import { url as base } from './url'

export const getUserToken = token => {
  const url = base + '/Login/google?googleTokenId=' + token

  return fetch(url, { method: 'post' }).then(res => res.text())
}

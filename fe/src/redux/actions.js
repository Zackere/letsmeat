export const setUser = user => dispatch => {
  dispatch({
    type: 'SET_USER',
    payload: user,
  })
}

export const setUserToken = token => dispatch => {
  dispatch({
    type: 'SET_TOKEN',
    payload: token,
  })
}

export const logOut = () => dispatch => {
  dispatch({
    type: 'LOG_OUT',
  })
}

export const initialState = {
  isAuthenticated: false,
  user: null,
  token: null,
}

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case 'SET_USER':
      return {
        ...state,
        user: action.payload,
      }

    case 'SET_TOKEN':
      return {
        ...state,
        token: action.payload,
        isAuthenticated: true,
      }

    case 'LOG_OUT':
      return {
        ...state,
        token: null,
        isAuthenticated: false,
        user: null,
      }
      
    default:
      return state
  }
}

export default reducer
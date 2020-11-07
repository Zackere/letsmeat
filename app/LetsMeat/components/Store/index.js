import React, { createContext, useReducer } from 'react';

const initialState = {
  loading: true,
  user: {
    signedIn: false,
  },
};
const store = createContext(initialState);
const { Provider } = store;

const StateProvider = ({ children }) => {
  const [state, dispatch] = useReducer((state, action) => {
    switch (action.type) {
      case 'SET_USER': {
        const newState = {
          ...state,
          user: {
            ...state.user,
            signedIn: true,
            name: action.payload.user.name,
            email: action.payload.user.email,
            photo: action.payload.user.photo,
            googleToken: action.payload.idToken
          },
        };
        // console.log(action.payload)
        return newState;
      }
      case 'SET_LOADING': {
        const newState = {
          ...state,
          loading: true,
        };
        return newState;
      }
      case 'SET_LOADED': {
        const newState = {
          ...state,
          loading: false,
        };
        return newState;
      }
      default:
        throw new Error();
    }
  }, initialState);

  return <Provider value={{ state, dispatch }}>{children}</Provider>;
};

export { store, StateProvider };

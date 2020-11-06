import React, { createContext, useReducer } from 'react';

const initialState = {
  user: {
    signedIn: false,
    userObject: {},
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
            userObject: action.payload,
          },
        };
        console.log(newState);
        return newState;
      }
      default:
        throw new Error();
    }
  }, initialState);

  return <Provider value={{ state, dispatch }}>{children}</Provider>;
};

export { store, StateProvider };

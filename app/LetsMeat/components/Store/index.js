import React, { createContext, useReducer } from 'react';

const initialState = {
  loading: true,
  user: {
    signedIn: false,
  },
  group: {},
  event: {},
  invitations: []
};
const store = createContext(initialState);
const { Provider } = store;

const StateProvider = ({ children }) => {
  const [state, dispatch] = useReducer((state, action) => {
    switch (action.type) {
      case 'LOGOUT': {
        const newState = {
          ...state,
          user: {
            ...state.user,
            signedIn: false
          },
        };
        return newState;
      }
      case 'SET_USER': {
        const newState = {
          ...state,
          user: {
            ...state.user,
            signedIn: true,
            name: action.payload.user.name,
            email: action.payload.user.email,
            photo: action.payload.user.photo,
            googleToken: action.payload.idToken,
            token: action.payload.token,
            id: action.payload.id
          },
        };
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
      case 'SET_GROUP': {
        const newState = {
          ...state,
          group: action.payload,
        };
        return newState;
      }
      case 'SET_EVENT': {
        const newState = {
          ...state,
          event: action.payload,
        };
        return newState;
      }
      case 'SET_INVITATIONS': {
        const newState = {
          ...state,
          invitations: action.payload,
        };
        return newState;
      }
      case 'REMOVE_INVITATION': {
        const newState = {
          ...state,
          invitations: state.invitations.filter((i) => i.group_id !== action.groupId),
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

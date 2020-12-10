import React, { useContext } from 'react';
import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { GoogleSignin } from '@react-native-community/google-signin';
import { store } from '../Store';

const baseURL = 'https://letsmeatapi.azurewebsites.net/';

const post = (route) => {
  const axiosConfig = { baseURL, token };
};

// const get = (state, endpoint) => AsyncStorage.getItem('token').then((token) => {
//   console.log(token);
//   const axiosConfig = { baseURL, params: { token }};
//   return axios.get(endpoint, axiosConfig);
// });

const setUser = (userInfo) => {
  console.log('setting user');
  return getAPIToken(userInfo.idToken)
    .then((token) => {
    // console.log(token);
    // AsyncStorage.setItem('token', token.data)
    //   .then(() => {
    //     dispatch({ type: 'SET_USER', payload: { ...userInfo } });
    //   }).catch((err) => console.log(err));
    });
};

const logIn = () => GoogleSignin.hasPlayServices()
  .then(GoogleSignin.signIn)
  .then(setUser);

const get = ({ state }, endpoint) => {
  const axiosConfig = { baseURL, params: { token: state.user.token } };
  return axios.get(endpoint, axiosConfig);
};

const getAPIToken = (googleToken) => {
  const axiosConfig = { baseURL, params: { googleTokenId: googleToken } };
  return axios.post('/Login/google', null, axiosConfig);
};

const appendAPIToken = (userInfo) => {
  const axiosConfig = { baseURL, params: { googleTokenId: userInfo.idToken } };
  return axios.post('/Login/google', null, axiosConfig).then((token) => ({ ...userInfo, token: token.data }));
};

const appendUserID = (userInfo) => {
  const axiosConfig = { baseURL, params: { token: userInfo.token } };
  console.log(userInfo)
  console.log(axiosConfig)
  return axios.get('/Users/info', axiosConfig).then((response) => ({ ...userInfo, ...response.data }));
}
// const getMyUserInfo = () => get('/Users/info', undefined, )

const getGroups = ({ state }) => get({ state }, '/Users/info').then((response) => response.data.groups);

const createGroup = (name) => axios.post('https://letsmeatapi.azurewebsites.net/Groups/create?',);

export {
  createGroup, getAPIToken, getGroups, appendAPIToken, appendUserID
};

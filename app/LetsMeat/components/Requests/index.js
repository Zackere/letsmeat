import React, { useContext } from 'react';
import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { GoogleSignin } from '@react-native-community/google-signin';
import { store } from '../Store';

const baseURL = 'https://letsmeatapi.azurewebsites.net/';

// const post = (route) => {
//   const axiosConfig = { baseURL, token };
// };

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

const get = ({ state }, endpoint, params = undefined) => {
  const axiosConfig = { baseURL, params: { token: state.user.token, ...params } };
  return axios.get(endpoint, axiosConfig);
};

const post = ({ state }, endpoint, data) => {
  const axiosConfig = { baseURL, params: { token: state.user.token } };
  return axios.post(endpoint, data, axiosConfig);
};

// _delete because delete is a JS keyword
// eslint-disable-next-line no-underscore-dangle
const _delete = ({ state }, endpoint, data) => {
  const axiosConfig = { baseURL, data, params: { token: state.user.token } };
  console.log('DELETING');
  console.log(axiosConfig);
  return axios.delete(endpoint, axiosConfig);
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
  return axios.get('/Users/info', axiosConfig).then((response) => ({ ...userInfo, ...response.data }));
};

const extractData = (response) => response.data;

const getGroups = ({ state }) => get({ state }, '/Users/info').then(extractData).then((data) => data.groups);

const getGroupInfo = ({ state }, id) => get({ state }, '/Groups/info', { id }).then(extractData);

const createGroup = ({ state }, name) => post({ state }, '/Groups/create/', { name }).then(extractData);

const deleteGroup = ({ state }, id) => _delete({ state }, '/Groups/delete/', { id });

const leaveGroup = ({ state }, id) => post({ state }, '/Groups/leave/', { id });

const createEvent = ({ state }, groupId, name, deadline) => post({ state }, '/Events/create', { group_id: groupId, name, deadline });

const getEventInfo = ({ state }, eventId) => get({ state }, '/Events/info', { id: eventId }).then(extractData);

const searchUsers = ({ state }, name) => get({ state }, '/Users/search', { name }).then(extractData);

const getUsersInfo = ({ state }, ids) => post({ state }, '/Users/info', { ids: (Array.isArray(ids) ? ids : [ids]) }).then(extractData);

const sendInvitation = ({ state }, userId, groupId) => post({ state }, '/Invitations/send', { to_id: userId, group_id: groupId });

const getInvitations = ({ state }) => get({ state }, '/Invitations/get').then(extractData);

const rejectInvitation = ({ state }, groupId) => _delete({ state }, '/Invitations/reject', { group_id: groupId }).catch(console.log);

const joinGroup = ({ state }, groupId) => post({ state }, '/Groups/join', { id: groupId });

const acceptInvitation = joinGroup;

export {
  getAPIToken, appendAPIToken, appendUserID,
  createGroup, getGroupInfo, deleteGroup, getGroups, leaveGroup, joinGroup,
  createEvent, getEventInfo,
  searchUsers, getUsersInfo,
  sendInvitation, getInvitations, rejectInvitation, acceptInvitation
};

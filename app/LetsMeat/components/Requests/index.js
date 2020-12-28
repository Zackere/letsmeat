import React, { useContext } from 'react';
import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { GoogleSignin } from '@react-native-community/google-signin';
// import FormData from 'form-data';
import { store } from '../Store';
// import { forScaleFromCenterAndroid } from '@react-navigation/stack/lib/typescript/src/TransitionConfigs/CardStyleInterpolators';

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

const post = ({ state }, endpoint, data, params) => {
  const axiosConfig = { baseURL, params: { token: state.user.token, ...params } };
  return axios.post(endpoint, data, axiosConfig);
};

const patch = ({ state }, endpoint, data, params) => {
  const axiosConfig = { baseURL, params: { token: state.user.token, ...params } };
  return axios.patch(endpoint, data, axiosConfig);
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
  return axios.get('/Users/info', axiosConfig).then((response) => {
    console.log(response.data);
    return ({ ...userInfo, ...response.data });
  });
};

const extractData = (response) => response.data;

const getGroups = ({ state }) => get({ state }, '/Users/info').then(extractData).then((data) => data.groups);

const getGroupInfo = ({ state }, id) => get({ state }, '/Groups/info', { id }).then(extractData);

const createGroup = ({ state }, name) => post({ state }, '/Groups/create/', { name }).then(extractData);

const deleteGroup = ({ state }, id) => _delete({ state }, '/Groups/delete/', { id });

const leaveGroup = ({ state }, id) => post({ state }, '/Groups/leave/', { id });

const createEvent = ({ state }, groupId, name, deadline) => post({ state }, '/Events/create', { group_id: groupId, name, deadline });

const getEventInfo = ({ state }, eventId) => get({ state }, '/Events/info', { id: eventId }).then(extractData);

const updateEvent = ({ state }, event) => patch({ state }, '/Events/update', event).then(extractData);

const searchUsers = ({ state }, name) => get({ state }, '/Users/search', { name }).then(extractData);

const getUsersInfo = ({ state }, ids) => post({ state }, '/Users/info', { ids: (Array.isArray(ids) ? ids : [ids]) }).then(extractData);

const updatePrefs = ({ state }, prefs) => post({ state }, '/Users/update_prefs', { ...prefs });

const sendInvitation = ({ state }, userId, groupId) => post({ state }, '/Invitations/send', { to_id: userId, group_id: groupId });

const getInvitations = ({ state }) => get({ state }, '/Invitations/get').then(extractData);

const rejectInvitation = ({ state }, groupId) => _delete({ state }, '/Invitations/reject', { group_id: groupId }).catch(console.log);

const joinGroup = ({ state }, groupId) => post({ state }, '/Groups/join', { id: groupId });

const acceptInvitation = joinGroup;

const uploadImage = ({ state }, eventId, image) => {
  const getNameFromPath = (path) => {
    const parts = path.split('/');
    return parts[parts.length - 1];
  };
  const form = new FormData();
  form.append('file', { uri: image.path, type: image.mime, name: getNameFromPath(image.path) });
  fetch(`${baseURL}Images/upload?${new URLSearchParams({ token: state.user.token, event_id: eventId })}`, {
    method: 'POST',
    headers: {
      Accept: 'text/',
      'Content-Type': 'multipart/form-data',
    },
    body: form
  }).then((response) => response.json());
};

const getImagesInfo = ({ state }, ids) => post({ state }, '/Images/info', { image_ids: (Array.isArray(ids) ? ids : [ids]) }).then(extractData);

const getVote = ({ state }, eventId) => get({ state }, '/Votes/get', { event_id: eventId }).then(extractData);

const getVoteTimes = ({ state }, eventId) => getVote({ state }, eventId).then((data) => data.times);

const getVoteLocations = ({ state }, eventId) => getVote({ state }, eventId)
  .then((data) => data.locations);

const castVote = ({ state }, eventId, times, locations) => post({ state }, '/Votes/cast',
  {
    event_id: eventId,
    vote_information: {
      times,
      locations
    }
  });

const searchLocation = ({ state }, groupId, query, sessionToken) => get({ state },
  '/Locations/search',
  { group_id: groupId, query_string: query, sessiontoken: sessionToken })
  .then(extractData);

const createLocationGoogle = ({ state }, placeId, sessionToken) => post({ state }, '/Locations/create_from_gmaps', { place_id: placeId, sessiontoken: sessionToken }).then(extractData);

const createLocationCustom = ({ state }, groupId, name, address) => post({ state }, '/Locations/create_custom', { group_id: groupId, name, address }).then(extractData);

const getLocationsInfo = ({ state }, customIds, googleIds) => post({ state },
  '/Locations/info',
  {
    custom_location_ids: customIds,
    google_maps_location_ids: googleIds
  })
  .then(extractData);

export {
  getAPIToken, appendAPIToken, appendUserID,
  createGroup, getGroupInfo, deleteGroup, getGroups, leaveGroup, joinGroup,
  createEvent, getEventInfo, updateEvent,
  searchUsers, getUsersInfo, updatePrefs,
  sendInvitation, getInvitations, rejectInvitation, acceptInvitation,
  uploadImage, getImagesInfo,
  getVote, getVoteTimes, getVoteLocations, castVote,
  searchLocation, createLocationGoogle, createLocationCustom,
};

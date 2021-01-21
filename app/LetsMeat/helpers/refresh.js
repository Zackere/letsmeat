import {
  getGroupInfo, getGroupDebts
} from '../components/Requests';

export const refreshGroup = (state, dispatch) => Promise.all(
  [getGroupInfo({ state, dispatch }, state.group.id),
    getGroupDebts({ state, dispatch }, state.group.id)]
).then(([groupInfo, debtInfo]) => {
  dispatch({ type: 'SET_GROUP', payload: { ...groupInfo, ...debtInfo } });
});

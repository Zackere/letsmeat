import { Action, SET_THEME } from './actions';
import { Theme } from '../types';

type RootState = Theme | '';

export default function reducer(state: RootState = '', action: Action): RootState {
  switch (action.type) {
    case SET_THEME:
      return action.payload;
    default:
      return state;
  }
}

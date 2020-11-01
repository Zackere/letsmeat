import * as ls from 'local-storage';
import { Dispatch } from 'redux';
import { Theme } from '../types';
import { Thunk } from '../../../utils/types/redux';

export const SET_THEME = 'SET_THEME';

interface SetThemeAction {
  type: typeof SET_THEME,
  payload: Theme,
}

function setTheme(theme: Theme): SetThemeAction {
  return {
    type: SET_THEME,
    payload: theme,
  };
}

export function switchTheme(theme: Theme): Thunk {
  return (dispatch: Dispatch): void => {
    ls.set<Theme>('theme', theme);
    dispatch(setTheme(theme));
  };
}

export function detectUserPreference(): Thunk {
  return (dispatch: Dispatch): void => {
    const savedPreference = ls.get<Theme | undefined>('theme');
    const prefersDarkTheme = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    const theme = savedPreference || (prefersDarkTheme ? 'dark' : 'light');

    dispatch(setTheme(theme));
  };
}

export type Action = SetThemeAction;

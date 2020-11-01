import { Action } from 'redux';
import { GlobalState } from '../../store';
import { ThunkAction } from 'redux-thunk';

export type Thunk<T = void> = ThunkAction<T, GlobalState, unknown, Action<string>>;

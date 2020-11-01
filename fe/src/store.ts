import * as theme from './modules/theme';
import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';

declare global {
  interface Window { __REDUX_DEVTOOLS_EXTENSION_COMPOSE__?: (...args: any[]) => any }
}

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

const rootReducer = combineReducers({
  theme: theme.reducer,
});

export type GlobalState = ReturnType<typeof rootReducer>;

const store = createStore(
  rootReducer,
  undefined,
  composeEnhancers(applyMiddleware(thunk)),
);

export default store;

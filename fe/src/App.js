import React from 'react'

import { Route, Switch, BrowserRouter as Router } from 'react-router-dom'
import ProtectedRoute from './common/protected route/ProtectedRoute'
import { ToastProvider } from 'react-toast-notifications'

import Login from './pages/login/Login'
import Unauthorized from './pages/unauthorized/Unauthorized'
import Home from './pages/home/Home'
import Group from './pages/group/Group'

import { createStore, applyMiddleware, compose } from 'redux'
import reducer, { initialState } from './redux/reducer'
import thunk from 'redux-thunk'
import { Provider } from 'react-redux'
import Bar from './common/bar/Bar'

const store = createStore(
  reducer,
  initialState,
  compose(
    applyMiddleware(thunk),
    window.devToolsExtension ? window.devToolsExtension() : f => f
  )
)

function App() {
  return (
    <Provider store={store}>
      <Router>
        <ToastProvider placement="bottom-right">
          <Bar />
          <Switch>
            <Route exact path="/" component={Login} />
            <Route exact path="/unauthorized" component={Unauthorized} />
            <ProtectedRoute exact path="/home" component={Home} />
            <ProtectedRoute exact path="/group:id" component={Group} />
            <Route component={Login} />
          </Switch>
        </ToastProvider>
      </Router>
    </Provider>
  )
}

export default App

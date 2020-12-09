import React from 'react'

import { Route, Switch, BrowserRouter as Router } from 'react-router-dom'
import ProtectedRoute from './common/protected route/ProtectedRoute'

import Home from './pages/home/Home'
import Unauthorized from './pages/unauthorized/Unauthorized'
import Groups from './pages/groups/Groups'
import Group from './pages/group/Group'

import { createStore, applyMiddleware, compose } from 'redux'
import reducer, { initialState } from './redux/reducer'
import thunk from 'redux-thunk'
import { Provider } from 'react-redux'

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
        <Switch>
          <Route exact path="/" component={Home} />
          <Route exact path="/unauthorized" component={Unauthorized} />
          <ProtectedRoute exact path="/groups" component={Groups} />
          <ProtectedRoute exact path="/group:id" component={Group} />
          <Route component={Home} />
        </Switch>
      </Router>
    </Provider>
  )
}

export default App

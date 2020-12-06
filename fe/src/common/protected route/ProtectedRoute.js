import React from 'react'
import { Route, Redirect } from 'react-router-dom'
import { connect } from 'react-redux'

const ProtectedRoute = ({ isAuth, component: Component, ...rest }) => {
  return (
    <Route
      {...rest}
      render={props =>
        isAuth ? (
          <Component {...rest} {...props} />
        ) : (
          <Redirect to={'/unauthorized'} />
        )
      }
    />
  )
}

const mapStateToProps = state => ({
  isAuth: state.isAuthenticated,
})

export default connect(mapStateToProps, null)(ProtectedRoute)

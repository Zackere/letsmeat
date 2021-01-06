import React, { Component } from 'react'
import { connect } from 'react-redux'
import { setUser, setUserToken } from '../../redux/actions'
import { withRouter } from 'react-router-dom'
import { GoogleLogin } from 'react-google-login'
import { getUserToken, getUser } from '../../services/userService'
import { Button } from 'react-bootstrap'
import Loading from '../../common/loading/Loading'
import './Login.css'

import '../../common/styles/styles.css'

class Login extends Component {
  state = {
    loading: false,
  }

  responseGoogle = response => {
    this.setState({ loading: true })

    getUserToken(response.tokenObj.id_token)
      .then(token => {
        this.props.setUserToken(token)
        return token
      })
      .then(token =>
        getUser(token).then(user => {
          this.props.setUser(user)
          this.setState({ loading: false })
          this.props.history.push('/home')
        })
      )
  }

  render() {
    return (
      <div className="row mx-0 background-image" style={{ height: '94vh' }}>
        <Loading show={this.state.loading} />
        <div className="centralizedContainer">
          <GoogleLogin
            clientId="1093858916900-rrqh9ehvffdtqblj6shfc7l8bbne08ih.apps.googleusercontent.com"
            onSuccess={this.responseGoogle}
            cookiePolicy={'single_host_origin'}
            render={renderProps => (
              <Button
                onClick={renderProps.onClick}
                variant="dark"
                style={{
                  background: '#343a40',
                  width: '200px',
                  height: '50px',
                  border: 'white solid 1px',
                  fontFamily: 'Segoe UI',
                }}
              >
                Start with Google
              </Button>
            )}
          />
        </div>
      </div>
    )
  }
}

const mapDispatchToProps = dispatch => ({
  setUser: user => dispatch(setUser(user)),
  setUserToken: token => dispatch(setUserToken(token)),
})

export default withRouter(connect(null, mapDispatchToProps)(Login))

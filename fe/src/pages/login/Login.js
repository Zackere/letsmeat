import React, { Component } from 'react'
import { connect } from 'react-redux'
import { setUser, setUserToken } from '../../redux/actions'
import { withRouter } from 'react-router-dom'
import { GoogleLogin } from 'react-google-login'
import { getUserToken } from '../../services/userService'
import { Container } from 'react-bootstrap'
import Loading from '../../common/loading/Loading'
import './Login.css'

class Login extends Component {

  state={
    loading: false
  }
  responseGoogle = response => {
    this.setState({loading: true})
    this.props.setUser(response.profileObj)

    getUserToken(response.tokenObj.id_token)
      .then(token => this.props.setUserToken(token))
      .then(() => {
        this.setState({loading: false})
        this.props.history.push('/home')
      })
  }

  render() {
    return (
      <Container>
        <Loading show={this.state.loading}/>
        <div className="centralizedContainer">
          <GoogleLogin
            clientId="1093858916900-rrqh9ehvffdtqblj6shfc7l8bbne08ih.apps.googleusercontent.com"
            onSuccess={this.responseGoogle}
            cookiePolicy={'single_host_origin'}
          />
        </div>
      </Container>
    )
  }
}

const mapDispatchToProps = dispatch => ({
  setUser: user => dispatch(setUser(user)),
  setUserToken: token => dispatch(setUserToken(token)),
})

export default withRouter(connect(null, mapDispatchToProps)(Login))

import React, { Component } from 'react'
import { connect } from 'react-redux'
import { setUser, setUserToken } from '../../redux/actions'
import { withRouter } from 'react-router-dom'
import { GoogleLogin } from 'react-google-login'
import { getUserToken } from '../../services/userServices'
import { Container } from 'react-bootstrap'
import Bar from '../../common/bar/Bar'
import './Home.css'

class Home extends Component {
  responseGoogle = response => {
    this.props.setUser(response.profileObj)

    getUserToken(response.tokenObj.id_token)
      .then(token => this.props.setUserToken(token))
      .then(() => this.props.history.push('/groups'))
  }

  render() {
    return (
      <Container>
        <Bar />
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

export default withRouter(connect(null, mapDispatchToProps)(Home))

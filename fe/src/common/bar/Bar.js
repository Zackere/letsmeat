import React, { Component } from 'react'
import { Navbar, Form, Button } from 'react-bootstrap'
import { connect } from 'react-redux'
import { IoIosLogOut } from 'react-icons/io'
import { IconContext } from 'react-icons'
import { logOut } from '../../redux/actions'
import { withRouter } from 'react-router-dom'

import 'bootstrap/dist/css/bootstrap.min.css'

class Bar extends Component {
  toUpper(str) {
    return str
      .toLowerCase()
      .split(' ')
      .map(word => word[0].toUpperCase() + word.substr(1))
      .join(' ')
  }
  onLogOut() {
    this.props.logOut()
    this.props.history.push('/')
  }

  render() {
    return (
      <Navbar bg="dark" variant="dark" expand="lg">
        <Navbar.Brand href="home">Let's vomeat</Navbar.Brand>
        <Navbar.Toggle />
        <Navbar.Collapse className="justify-content-end">
          <Navbar.Text>
            <a>
              {this.props.isAuth ? (
                <img
                  src={this.props.user.imageUrl}
                  width="36px"
                  className="mr-3 rounded-circle"
                />
              ) : (
                <></>
              )}
              {this.props.isAuth ? this.toUpper(this.props.user.givenName) : ''}
            </a>
          </Navbar.Text>
          <Form inline>
            <Button variant="link" onClick={() => this.onLogOut()}>
              <IconContext.Provider value={{ size: '30px', color: 'white' }}>
                <IoIosLogOut />
              </IconContext.Provider>
            </Button>
          </Form>
        </Navbar.Collapse>
      </Navbar>
    )
  }
}
const mapStateToProps = state => ({
  isAuth: state.isAuthenticated,
  user: state.user,
})

const mapDispatchToProps = dispatch => ({
  logOut: () => dispatch(logOut()),
})

export default withRouter(connect(mapStateToProps, mapDispatchToProps)(Bar))

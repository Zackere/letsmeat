import React, { Component } from 'react'
import { Navbar, Form, Button } from 'react-bootstrap'
import { connect } from 'react-redux'
import { HiLogout } from 'react-icons/hi'
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
      <Navbar
        bg="dark"
        variant="dark"
        expand="lg"
        className="px-5"
        style={{ height: '6vh' }}
      >
        <Navbar.Brand href="/">Let's vomeat</Navbar.Brand>
        <Navbar.Toggle />
        <Navbar.Collapse className="justify-content-end">
          <Form inline>
            <Button variant="link" onClick={() => this.onLogOut()}>
              <IconContext.Provider value={{ size: '30px', color: 'white' }}>
                <HiLogout />
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

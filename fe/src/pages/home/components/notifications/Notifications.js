import React, { Component } from 'react'
import { connect } from 'react-redux'
import Notification from './components/Notification'
import { ListGroup } from 'react-bootstrap'

import { getInvitations } from '../../../../services/invitationService'

class Notifications extends Component {
  constructor(props) {
    super(props)
    this.state = {
      notifcations: [],
    }
  }

  componentDidMount() {
    getInvitations(this.props.token).then(notifcations => {
      this.setState({ notifcations })
      console.log(notifcations)
    })
  }

  toUpper(str) {
    return str
      .toLowerCase()
      .split(' ')
      .map(word => word[0].toUpperCase() + word.substr(1))
      .join(' ')
  }

  removeNotification = group_id => {
    const notifications = [...this.state.notifcations]

    const index = notifications.findIndex(n => n.group_id === group_id)
    notifications.splice(index, 1)

    this.setState({ notifications })
  }
  render() {
    return (
      <div className="w-100 h-100" style={{ backgroundColor: 'white' }}>
        <div className="row h-25 py-5 flex-column">
          <div className="d-flex justify-content-center mb-3">
            <img
              src={this.props.user.imageUrl}
              width="50px"
              height="50px"
              className="rounded-circle"
              alt=""
            />
          </div>
          <div className="d-flex justify-content-center mb-2">
            <h6>{this.toUpper(this.props.user.name)}</h6>
          </div>
          <div className="d-flex justify-content-center">
            <span>{this.props.user.email}</span>
          </div>
        </div>
        <div className="flex-column h-75 p-5 mx-0 border-top">
          <h5 className="mb-4">Notifications</h5>
          <ListGroup
            className="list-scroll d-flex"
            style={{ maxHeight: '55vh', height: '100%' }}
          >
            {this.state.notifcations.map(n => (
              <ListGroup.Item
                className="rounded-0 border-left-0"
                key={n.group_id}
              >
                <Notification
                  from_id={n.from_id}
                  group_id={n.group_id}
                  removeNotification={this.removeNotification}
                />
              </ListGroup.Item>
            ))}
            {this.state.notifcations.length > 0 ? (
              <div />
            ) : (
              <span className="align-self-center">You have no notifications</span>
            )}
          </ListGroup>
        </div>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  user: state.user,
  token: state.token,
})
export default connect(mapStateToProps, null)(Notifications)

import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Button } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'

import Loading from '../../../../../common/loading/Loading'
import {success, error} from '../../../../../common/toasts/toasts'

import { getUsers } from '../../../../../services/userService'
import { getGroup, joinGroup } from '../../../../../services/groupsService'

class Notification extends Component {
  constructor(props) {
    super(props)
    this.state = {
      groupName: '',
      userName: '',
      loading: false,
    }
  }

  componentDidMount() {
    this.setState({ loading: true })

    getUsers(this.props.token, [this.props.from_id])
      .then(users => this.setState({ userName: users[0].name }))
      .then(() => getGroup(this.props.token, this.props.group_id))
      .then(group => this.setState({ groupName: group.name }))
      .then(() => this.setState({ loading: false }))
  }

  joinGroup = () => {
    joinGroup(this.props.token, this.props.group_id).then(res => {
      if (res.ok) {
        this.props.removeNotification(this.props.group_id)
        success(
          'Now you are a member of ' + this.state.groupName,
          this.props.toastManager
        )
      } else {
        error('Something went wrong', this.props.toastManager)
      }
    })
  }
  render() {
    return (
      <div className="flex-column w-100 h-100">
        <Loading show={this.state.loading} />
        <p>
          <b> {this.state.userName}</b> invites you to join group{' '}
          <b> {this.state.groupName}</b>
        </p>
        <div className="d-flex float-right">
          <Button variant="outline-danger" className="mr-2 border-0">
            Refuse
          </Button>
          <Button variant="outline-primary" className="ml-2 border-0" onClick={this.joinGroup}>
            Accept
          </Button>
        </div>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})
export default withToastManager(connect(mapStateToProps, null)(Notification))

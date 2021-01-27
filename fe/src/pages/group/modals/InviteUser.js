import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button } from 'react-bootstrap'
import UserSearch from '../components/UserSearch'
import { withToastManager } from 'react-toast-notifications'

import 'bootstrap/dist/css/bootstrap.min.css'
import { sendInvitation } from '../../../services/invitationService'
import { success, error } from '../../../common/toasts/toasts'

class InviteUser extends Component {
  constructor(props) {
    super(props)

    this.state = {
      selected: []
    }
  }

  closeModal = () => {
    this.setState({ selected: [] })
    this.props.closeModal()
  }

  inviteUsers = () => {
    for (let user of this.state.selected) {
      sendInvitation(this.props.token, this.props.groupId, user.id).then(
        res => {
          if (res.ok) {
            success(user.name + ' got your invitation', this.props.toastManager)
          }

          else{
            error('You cannot invite ' + user.name, this.props.toastManager)
          }
        }
      )
    }
    this.closeModal();
  }

  setSelectedUsers = users => this.setState({ selected: users })

  render() {
    return (
      <Modal
        show={this.props.show}
        aria-labelledby="contained-modal-title-vcenter"
        onHide={this.closeModal}
        centered
      >
        <Modal.Header closeButton>
          <Modal.Title id="contained-modal-title-vcenter">
            Invite new member
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <UserSearch setSelected={this.setSelectedUsers} />
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.inviteUsers}>
            Send
          </Button>
        </Modal.Footer>
      </Modal>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(InviteUser))

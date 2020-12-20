import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'
import DatePicker from 'react-datepicker'

import { addEvent } from '../../../services/eventService'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'react-datepicker/dist/react-datepicker.css'

import { success, error } from '../../../common/toasts/toasts'

class InviteUser extends Component {
  constructor(props) {
    super(props)

    this.state = {
      name: '',
      date: new Date(),
    }
  }

  nameChanged = event => {
    this.setState({ name: event.target.value })
  }

  dateChanged = date => {
    this.setState({ date })
  }

  closeModal = () => {
    this.setState({ name: '' })
    this.props.closeModal()
  }

  createEvent = () => {
    addEvent(
      this.state.name,
      this.state.date,
      this.props.groupId,
      this.props.token
    )
      .then(id => {
        const event = {
          id,
          name: this.state.name,
          deadline: this.state.date,
        }

        this.props.addEvent(event)

        success(
          'Event ' + event.name + ' successfully created',
          this.props.toastManager
        )
        this.closeModal()
      })
      .catch(e => {
        error('Failed to create event', this.props.toastManager)
        this.closeModal()
      })
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
            Add new event
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="row">
            <div className="col-12">
              <Form>
                <Form.Group controlId="formBasicEmail">
                  <Form.Label>Event name</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Enter name"
                    value={this.state.name}
                    onChange={this.nameChanged}
                  />
                </Form.Group>
              </Form>
            </div>
          </div>
          <label className="mt-2">Voting deadline</label>
          <div className="row">
            <div className="col-12">
              <DatePicker
                selected={this.state.date}
                onChange={this.dateChanged}
                showTimeSelect
                timeFormat="HH:mm"
                timeIntervals={30}
                timeCaption="time"
                dateFormat="dd MMMM yyyy, HH:mm"
              />
            </div>
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.createEvent}>
            Add
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

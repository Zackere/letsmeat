import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'
import DatePicker from 'react-datepicker'

import { addEvent } from '../../../services/eventService'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'react-datepicker/dist/react-datepicker.css'

import { success, error } from '../../../common/toasts/toasts'

class AddEvent extends Component {
  constructor(props) {
    super(props)

    const date = new Date()
    date.setMinutes(Math.ceil(date.getMinutes() / 10) * 10, 0, 0)

    this.state = {
      name: '',
      date,
      isInvalid: false,
    }
  }

  nameChanged = event => {
    this.setState({ name: event.target.value, isInvalid: false })
  }

  dateChanged = date => {
    this.setState({ date })
  }

  closeModal = () => {
    this.setState({ name: '', isInvalid: false })
    this.props.closeModal()
  }

  isInvalid = () => {
    const name = this.state.name
    const isInvalid = name.length <= 0 || name.length > 25
    this.setState({ isInvalid })
    return isInvalid
  }

  createEvent = () => {
    if (this.isInvalid()) return

    addEvent(
      this.state.name,
      this.state.date,
      this.props.groupId,
      this.props.token
    )
      .then(res => {
        const event = {
          id: res.id,
          name: this.state.name,
          deadline: this.state.date,
          creator_id: this.props.user.id,
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

  timeFilter = time => {
    const currentDate = new Date(time)
    const selectedDate = this.state.date

    if (selectedDate.getDate() === new Date().getDate()) {
      return currentDate.getTime() >= selectedDate.getTime()
    }
    return currentDate.getTime() < selectedDate.getTime()
  }

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
                    isInvalid={this.state.isInvalid}
                  />
                  <Form.Control.Feedback type="invalid">
                    {this.state.name.length === 0
                      ? 'Event name cannot be empty'
                      : 'Lenght cannot exceed 25 characters'}
                  </Form.Control.Feedback>
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
                timeIntervals={10}
                filterTime={this.timeFilter}
                minDate={new Date()}
                timeCaption="Time"
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
  user: state.user,
})

export default withToastManager(connect(mapStateToProps, null)(AddEvent))

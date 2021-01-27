import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'
import DatePicker from 'react-datepicker'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'react-datepicker/dist/react-datepicker.css'

class DateModal extends Component {
  constructor(props) {
    super(props)

    this.state = {
      date: props.deadline,
    }
  }

  dateChanged = date => this.setState({ date })

  closeModal = () => this.props.closeModal()

  addDate = () => {
    this.props.closeModal()
    this.props.addDate(this.state.date)
  }


  timeFilter = time => {
    //todo fix validation
    const currentDate = new Date(time)
    const selectedDate = this.state.date

    if (selectedDate.getDate() === this.props.deadline.getDate()) {
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
            Add new meeting time
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <label className="mt-2">Meeting time</label>
          <div className="col-12">
            <DatePicker
              selected={this.state.date}
              onChange={this.dateChanged}
              showTimeSelect
              minDate={this.props.deadline}
              filterTime={this.timeFilter}
              timeFormat="HH:mm"
              timeIntervals={10}
              timeCaption="time"
              dateFormat="dd MMMM yyyy, HH:mm"
            />
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.addDate}>
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

export default withToastManager(connect(mapStateToProps, null)(DateModal))

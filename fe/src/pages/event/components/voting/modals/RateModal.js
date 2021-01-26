import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'

import { success, error } from '../../../../../common/toasts/toasts'
import Place from '../components/Place'

import { getResult } from '../../../../../services/votingService'
import { rateLocation } from '../../../../../services/locationsService'
import { basicFormat } from '../../../../../common/date formater/dateFormater'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../../../../common/styles/styles.css'

class RateModal extends Component {
  constructor(props) {
    super(props)

    this.state = {
      price: 0,
      quality: 0,
      quantity: 0,
      waitingTime: 0,
      locationId: undefined,
      date: undefined,
    }
  }

  componentDidMount() {
    getResult(this.props.token, this.props.eventId).then(res =>
      this.setState({ locationId: res.locations[0], date: res.times[0] })
    )
  }

  rateLocation = () => {
    const { price, quality, quantity, waitingTime, locationId } = this.state

    const body = {
      taste: parseInt(quality),
      price: parseInt(price),
      amount_of_food: parseInt(quantity),
      waiting_time: parseInt(waitingTime),
      google_maps_id: locationId.google_maps_location_id,
      custom_location_id: locationId.custom_location_id,
    }

    this.closeModal()

    rateLocation(this.props.token, body).then(res => {
      if (res.ok) {
        success('Place rated successfully', this.props.toastManager)
      } else {
        error('Failed to rate this place', this.props.toastManager)
      }
    })
  }

  closeModal = () => {
    this.props.closeModal()
  }

  render() {
    const loc = this.state.locationId
    const date = this.state.date
    if (!loc || !date) return <div />

    return (
      <Modal
        show={this.props.show}
        aria-labelledby="contained-modal-title-vcenter"
        onHide={this.closeModal}
        centered
      >
        <Modal.Header closeButton>
          <Modal.Title id="contained-modal-title-vcenter">
            Meeting place and time
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Place
            googleId={loc?.google_maps_location_id}
            customId={loc?.custom_location_id}
            showRating={false}
          />
          <div className="row">
            <div className="col-1" />
            <div className="d-flex" style={{ paddingLeft: '15px' }}>
              <span className="small-string">{basicFormat(date)}</span>
            </div>
          </div>
          <h5 className="mt-2">Rate</h5>
          <Form>
            <Form.Group controlId="formBasicRange">
              <Form.Label>Price</Form.Label>
              <Form.Control
                type="range"
                value={this.state.price}
                onChange={e => this.setState({ price: e.target.value })}
              />
              <Form.Label>Quality</Form.Label>
              <Form.Control
                type="range"
                value={this.state.quality}
                onChange={e => this.setState({ quality: e.target.value })}
              />
              <Form.Label>Quantity</Form.Label>
              <Form.Control
                type="range"
                value={this.state.quantity}
                onChange={e => this.setState({ quantity: e.target.value })}
              />
              <Form.Label>Wating time</Form.Label>
              <Form.Control
                type="range"
                value={this.state.waitingTime}
                onChange={e => this.setState({ waitingTime: e.target.value })}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.rateLocation}>
            Rate
          </Button>
        </Modal.Footer>
      </Modal>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(RateModal))

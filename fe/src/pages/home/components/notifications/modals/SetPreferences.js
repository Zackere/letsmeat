import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'

import { success, error } from '../../../../../common/toasts/toasts'
import {
  getPreferences,
  setPreferences,
} from '../../../../../services/preferencesService'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../../../../common/styles/styles.css'

class SetPreferences extends Component {
  constructor(props) {
    super(props)

    this.state = {
      price: 0,
      quality: 0,
      quantity: 0,
      waitingTime: 0,
    }
  }

  componentDidMount() {
    getPreferences(this.props.token).then(prefs => {
      this.setState({
        price: prefs.price,
        quality: prefs.taste,
        quantity: prefs.amount_of_food,
        waitingTime: prefs.waiting_time,
      })
    })
  }

  savePreferences = () => {
    const { price, quality, quantity, waitingTime } = this.state

    const prefs = {
      taste: parseInt(quality),
      price: parseInt(price),
      amount_of_food: parseInt(quantity),
      waiting_time: parseInt(waitingTime),
    }
    
    this.closeModal()
    
    setPreferences(this.props.token, prefs).then(res => {
      if (res.ok) {
        success('New preferences set successfully', this.props.toastManager)
      } else {
        error('Failed to set preferences', this.props.toastManager)
      }
    })
  }

  closeModal = () => {
    this.props.closeModal()
  }

  render() {
    return (
      <Modal
        show={this.props.show}
        size="sm"
        aria-labelledby="contained-modal-title-vcenter"
        onHide={this.closeModal}
        centered
      >
        <Modal.Header closeButton>
          <Modal.Title id="contained-modal-title-vcenter">
            Preferences
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
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
          <Button variant="outline-primary" onClick={this.savePreferences}>
            Set
          </Button>
        </Modal.Footer>
      </Modal>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(SetPreferences))

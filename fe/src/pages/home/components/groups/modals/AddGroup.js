import React, { Component } from 'react'

import { Modal, Form, Button } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

export default class AddGroup extends Component {
  constructor(props) {
    super(props)

    this.state = {
      name: '',
      isInvalid: false,
    }
  }

  nameChanged = event =>
    this.setState({ name: event.target.value, isInvalid: false })

  addGroup = () => {
    if (this.isInvalid()) return

    this.props.addGroup(this.state.name)
    this.closeModal()
    //toast
  }

  closeModal = () => {
    this.setState({ name: '', isInvalid: false })
    this.props.onClose()
  }

  isInvalid = () => {
    const name = this.state.name
    const isInvalid = name.length <= 0 || name.length > 25
    this.setState({ isInvalid })
    return isInvalid
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
            Add new group
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="formName">
              <Form.Label>Group name</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter name"
                value={this.state.name}
                onChange={this.nameChanged}
                isInvalid={this.state.isInvalid}
              />
              <Form.Control.Feedback type="invalid">
                {this.state.name.length === 0
                  ? 'Grupe name cannot be empty'
                  : 'Lenght cannot exceed 25 characters'}
              </Form.Control.Feedback>
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.addGroup}>
            Add
          </Button>
        </Modal.Footer>
      </Modal>
    )
  }
}

import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Form, Button, Modal } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'react-datepicker/dist/react-datepicker.css'

class CustomPlaceModal extends Component {
  constructor(props) {
    super(props)

    this.state = {
      name: '',
      address: '',
      addressIsInvalid: false,
      nameIsInvalid: false,
    }
  }

  nameIsInvalid = () => {
    const name = this.state.name
    const nameIsInvalid = name.length <= 0 || name.length > 25

    this.setState({ nameIsInvalid })
    return nameIsInvalid
  }

  addressIsInvalid = () => {
    const address = this.state.address
    const addressIsInvalid = address.length <= 0 || address.length > 25

    this.setState({ addressIsInvalid })
    return addressIsInvalid
  }

  closeModal = () => {
    this.setState({
      name: '',
      address: '',
      addressIsInvalid: false,
      nameIsInvalid: false,
    })
    this.props.closeModal()
  }

  addPlace = () => {
    const addressIsInvalid = this.addressIsInvalid()
    const nameIsInvalid = this.nameIsInvalid()

    if (addressIsInvalid || nameIsInvalid) return

    this.props.addCustomPlace(this.state.name, this.state.address)
    this.closeModal()
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
            Add new meeting place
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <label className="mt-2">Name</label>
          <div className="col-12">
            <Form>
              <Form.Group>
                <Form.Control
                  type="text"
                  placeholder="Name"
                  onChange={e =>
                    this.setState({
                      name: e.target.value,
                      nameIsInvalid: false,
                    })
                  }
                  isInvalid={this.state.nameIsInvalid}
                />
                <Form.Control.Feedback type="invalid">
                  {this.state.name.length === 0
                    ? 'Name cannot be empty'
                    : 'Lenght cannot exceed 25 characters'}
                </Form.Control.Feedback>
              </Form.Group>
            </Form>
          </div>
          <label className="mt-2">Address</label>
          <div className="col-12">
            <Form>
              <Form.Group>
                <Form.Control
                  type="text"
                  value={this.state.address}
                  placeholder="Address"
                  onChange={e =>
                    this.setState({
                      address: e.target.value,
                      addressIsInvalid: false,
                    })
                  }
                  isInvalid={this.state.addressIsInvalid}
                />
                <Form.Control.Feedback type="invalid">
                  {this.state.name.length === 0
                    ? 'Address cannot be empty'
                    : 'Lenght cannot exceed 35 characters'}
                </Form.Control.Feedback>
              </Form.Group>
            </Form>
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.addPlace}>
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

export default withToastManager(
  connect(mapStateToProps, null)(CustomPlaceModal)
)

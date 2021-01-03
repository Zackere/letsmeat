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
    }
  }

  closeModal = () => {
    this.setState({ name: '', address: '' })
    this.props.closeModal()
  }

  addPlace = () => {
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
          <label className="mt-2">Meeting place</label>
          <div className="col-12">
            <Form>
              <Form.Group>
                  <Form.Control
                    type="text"
                    placeholder="Name"
                    onChange={e => this.setState({name: e.target.value})}
                  />
              </Form.Group>
            </Form>
          </div>
          <label className="mt-2">Meeting address</label>
          <div className="col-12">
            <Form>
              <Form.Group>
                  <Form.Control
                    type="text"
                    value={this.state.address}
                    placeholder="Address"
                    onChange={e => this.setState({address: e.target.value})}
                  />
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

import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'

import 'bootstrap/dist/css/bootstrap.min.css'

class ShowBalance extends Component {
  constructor(props) {
    super(props)

    this.state = {
      message: '',
      amount: 0,
    }
  }

  closeModal = () => {
    this.setState({ message: '', amount: 0 })
    this.props.closeModal()
  }

  sendMoney = () => {
    this.closeModal()
    this.props.sendMoney(this.state.amount * 100, this.state.message)
  }

  render() {
    const debt = this.props.debt
    const user = this.props.user

    return (
      <Modal
        show={this.props.show}
        aria-labelledby="contained-modal-title-vcenter"
        onHide={this.closeModal}
        centered
      >
        <Modal.Header closeButton>
          <Modal.Title id="contained-modal-title-vcenter">
            {(debt >= 0 ? 'You owe ' + user.name : user.name + ' owes you') +
              ' ' +
              Math.abs(debt) +
              'zł'}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="flex-column">
            <h5>Transaction</h5>
            <Form>
              <Form.Group>
                <Form.Label>Amount</Form.Label>
                <Form.Control
                  type="number"
                  min={0}
                  placeholder="Enter amount you want to send"
                  value={this.state.amount}
                  onChange={e => this.setState({ amount: e.target.value })}
                />
              </Form.Group>
              <Form.Group>
                <Form.Label>Message</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter message"
                  value={this.state.message}
                  onChange={e => this.setState({ message: e.target.value })}
                />
              </Form.Group>
            </Form>
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="outline-secondary" onClick={this.closeModal}>
            Close
          </Button>
          <Button variant="outline-primary" onClick={this.sendMoney}>
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

export default withToastManager(connect(mapStateToProps, null)(ShowBalance))

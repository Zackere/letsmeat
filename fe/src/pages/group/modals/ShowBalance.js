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
      amountIsInvalid: false,
      messageIsInvalid: false,
    }
  }

  closeModal = () => {
    this.setState({
      message: '',
      amount: 0,
      amountIsInvalid: false,
      messageIsInvalid: false,
    })
    this.props.closeModal()
  }

  sendMoney = () => {
    const messageIsInvalid = this.messageIsInvalid()
    const amountIsInvalid = this.amountIsInvalid()
    
    if (messageIsInvalid || amountIsInvalid) return

    this.closeModal()
    this.props.sendMoney(this.state.amount * 100, this.state.message)
  }

  amountIsInvalid = () => {
    const amount = parseInt(this.state.amount)
    const amountIsInvalid = isNaN(amount) || amount <= 0

    this.setState({ amountIsInvalid })
    return amountIsInvalid
  }

  messageIsInvalid = () => {
    const message = this.state.message
    const messageIsInvalid = message.length <= 0 || message.length > 35

    this.setState({ messageIsInvalid })
    return messageIsInvalid
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
                  onChange={e =>
                    this.setState({
                      amount: e.target.value,
                      amountIsInvalid: false,
                    })
                  }
                  isInvalid={this.state.amountIsInvalid}
                />
                <Form.Control.Feedback type="invalid">
                  Amount must be possitive
                </Form.Control.Feedback>
              </Form.Group>
              <Form.Group>
                <Form.Label>Message</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter message"
                  value={this.state.message}
                  onChange={e =>
                    this.setState({
                      message: e.target.value,
                      messageIsInvalid: false,
                    })
                  }
                  isInvalid={this.state.messageIsInvalid}
                />
                <Form.Control.Feedback type="invalid">
                  {this.state.message.length === 0
                    ? 'Message cannot be empty'
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

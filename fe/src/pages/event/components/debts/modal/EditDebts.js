import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'
import { IconContext } from 'react-icons'
import { IoIosArrowForward, IoIosArrowBack } from 'react-icons/io'

import Loading from '../../../../../common/loading/Loading'

import { updateImageDebt } from '../../../../../services/imageService'
import { addDebt } from '../../../../../services/debtService'

import 'bootstrap/dist/css/bootstrap.min.css'

class EditDebts extends Component {
  constructor(props) {
    super(props)

    this.state = {
      debts: props.debts,
      assignedTo: [],
      index: 0,
      assignedUser: undefined,
      loading: false,
    }
  }

  componentDidMount() {
    this.updateAssignedTo()
  }

  componentDidUpdate(prevProps) {
    if (this.props.debts !== prevProps.debts && !this.state.loading) {
      this.setState({ debts: this.props.debts })
      this.updateAssignedTo()
    }
  }

  updateAssignedTo = () => {
    const assignedTo = []
    const debts = [...this.props.debts].map(debt => {
      const d = Object.assign({}, debt)
      d.amount = d.amount / 100
      return d
    })

    const index = debts.findIndex(debt => debt.id === this.props.selected.id)

    for (const debt of debts) {
      debt.pending_debt
        ? assignedTo.push(debt.pending_debt.from_id)
        : assignedTo.push(0)
    }

    this.setState({ assignedTo, debts, index })
  }

  closeModal = () => {
    this.setState({ index: 0 })
    this.props.closeModal()
  }

  setAmount = amount => {
    const debts = [...this.state.debts]
    debts[this.state.index].amount = amount
    this.setState({ debts })
  }

  setDescription = description => {
    const debts = [...this.state.debts]
    debts[this.state.index].description = description
    this.setState({ debts })
  }

  setAssignedTo = user_id => {
    const assignedTo = [...this.state.assignedTo]
    assignedTo[this.state.index] = user_id
    this.setState({ assignedTo })
  }

  indexUp = () => {
    const index = Math.min(this.state.index + 1, this.state.debts.length - 1)
    this.setState({ index })
  }

  indexDown = () => {
    const index = Math.max(this.state.index - 1, 0)
    this.setState({ index })
  }

  saveDebt = () => {
    const debts = this.state.debts
    const index = this.state.index
    const assignedTo = this.state.assignedTo

    this.setState({ loading: true })

    const imageDebt = {
      id: debts[index].id,
      amount: parseInt(debts[index].amount * 100),
      description: debts[index].description,
    }

    const debt = {
      event_id: this.props.eventId,
      group_id: this.props.groupId,
      from_id: assignedTo[index],
      to_id: this.props.user_id,
      amount: parseInt(debts[index].amount * 100),
      description: debts[index].description,
      image_debt_id: debts[index].id,
      debt_type: 0,
    }

    if (!assignedTo[index]) {
      updateImageDebt(this.props.token, imageDebt).then(() => {
        this.props.getDebts()
        this.setState({ loading: false })
      })
    } else {
      addDebt(this.props.token, debt).then(() => {
        updateImageDebt(this.props.token, imageDebt).then(() => {
          this.props.getDebts()
          this.setState({ loading: false })
        })
      })
    }
  }

  render() {
    const debts = this.state.debts
    const index = this.state.index
    const assignedTo = this.state.assignedTo

    return (
      <>
        <Loading show={this.state.loading} />
        <Modal
          show={this.props.show}
          aria-labelledby="contained-modal-title-vcenter"
          onHide={this.closeModal}
          centered
        >
          <Modal.Header closeButton>
            <Modal.Title id="contained-modal-title-vcenter"></Modal.Title>
            <div className="row d-flex justify-content-center w-100">
              <Button
                variant="link"
                className="no-focus"
                onClick={this.indexDown}
              >
                <IconContext.Provider
                  value={{ size: '23px', color: '#343a40' }}
                >
                  <IoIosArrowBack />
                </IconContext.Provider>
              </Button>
              <div style={{ lineHeight: '50px', height: '50px' }}>
                {index + 1 + '/' + debts.length}
              </div>
              <Button
                variant="link"
                className="no-focus"
                onClick={this.indexUp}
              >
                <IconContext.Provider
                  value={{ size: '23px', color: '#343a40' }}
                >
                  <IoIosArrowForward />
                </IconContext.Provider>
              </Button>
            </div>
          </Modal.Header>
          <Modal.Body>
            <div className="flex-column">
              <h5>Debt</h5>
              <Form>
                <Form.Group>
                  <Form.Label>Amount</Form.Label>
                  <Form.Control
                    type="number"
                    min={0}
                    placeholder="Enter amount you paid"
                    value={debts[index].amount}
                    onChange={e => this.setAmount(e.target.value)}
                  />
                </Form.Group>
                <Form.Group>
                  <Form.Label>Description</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Enter description"
                    value={debts[index].description}
                    onChange={e => this.setDescription(e.target.value)}
                  />
                </Form.Group>
                <Form.Group>
                  <Form.Label>Assigned to</Form.Label>
                  <Form.Control
                    as="select"
                    value={assignedTo[index]}
                    onChange={e => this.setAssignedTo(e.target.value)}
                  >
                    <option disabled value={0}>
                      Not assigned
                    </option>
                    {this.props.users.map(u => (
                      <option value={u.id}>{u.name}</option>
                    ))}
                  </Form.Control>
                </Form.Group>
              </Form>
            </div>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="outline-secondary" onClick={this.closeModal}>
              Close
            </Button>
            <Button variant="outline-primary" onClick={this.saveDebt}>
              Save
            </Button>
          </Modal.Footer>
        </Modal>
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
  user_id: state.user.id,
})

export default withToastManager(connect(mapStateToProps, null)(EditDebts))

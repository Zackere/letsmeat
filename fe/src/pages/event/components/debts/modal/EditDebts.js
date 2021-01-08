import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Modal, Button, Form } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'
import { IconContext } from 'react-icons'
import { IoIosArrowForward, IoIosArrowBack } from 'react-icons/io'
import { BsFillPlusCircleFill } from 'react-icons/bs'

import Loading from '../../../../../common/loading/Loading'

import {
  updateImageDebt,
  addImageDebt,
  getImages,
} from '../../../../../services/imageService'
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
      showReceipt: false,
      imageUrl: '',
      width: 0,
      amountIsInvalid: false,
      descriptionIsInvalid: false,
    }
  }

  _modalBody = React.createRef()

  componentDidUpdate(prevProps) {
    if (this.props.debts !== prevProps.debts && !this.state.loading) {
      const width = this._modalBody?.current?.clientWidth
      this.setState({ width, loading: true })

      getImages(this.props.token, [this.props.debts[0].image_id]).then(images =>
        this.updateState(images[0].image_url)
      )
    }
  }

  updateState = imageUrl => {
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

    this.setState({ assignedTo, debts, index, imageUrl, loading: false })
  }

  closeModal = () => {
    this.setState({ index: 0, showReceipt: false })
    this.props.closeModal()
  }

  setAmount = amount => {
    const debts = [...this.state.debts]
    debts[this.state.index].amount = amount
    this.setState({ debts, amountIsInvalid: false })
  }

  setDescription = description => {
    const debts = [...this.state.debts]
    debts[this.state.index].description = description
    this.setState({ debts, descriptionIsInvalid: false })
  }

  setAssignedTo = user_id => {
    const assignedTo = [...this.state.assignedTo]
    assignedTo[this.state.index] = user_id
    this.setState({ assignedTo })
  }

  indexUp = () => {
    const index = this.state.index + 1
    this.setState({
      index,
      descriptionIsInvalid: false,
      amountIsInvalid: false,
    })
  }

  indexDown = () => {
    const index = this.state.index - 1
    this.setState({
      index,
      descriptionIsInvalid: false,
      amountIsInvalid: false,
    })
  }

  amountIsInvalid = () => {
    const debts = this.state.debts
    const index = this.state.index
    const amount = parseInt(debts[index].amount)
    const amountIsInvalid = isNaN(amount) || amount <= 0

    this.setState({ amountIsInvalid })
    return amountIsInvalid
  }

  descriptionIsInvalid = () => {
    const debts = this.state.debts
    const index = this.state.index
    const description = debts[index].description

    const descriptionIsInvalid =
      description.length <= 0 || description.length > 35

    this.setState({ descriptionIsInvalid })
    return descriptionIsInvalid
  }

  saveDebt = () => {
    const debts = this.state.debts
    const index = this.state.index
    const assignedTo = this.state.assignedTo
    const descriptionIsInvalid = this.descriptionIsInvalid()
    const amountIsInvalid = this.amountIsInvalid()

    if (descriptionIsInvalid || amountIsInvalid) return

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
        this.props.getDebts(false, debts[index])
        this.setState({ loading: false })
      })
    } else {
      addDebt(this.props.token, debt).then(() => {
        updateImageDebt(this.props.token, imageDebt).then(() => {
          this.props.getDebts(false, debts[index])
          this.setState({ loading: false })
        })
      })
    }
  }

  addDebt = () => {
    const debts = this.state.debts
    const index = this.state.index

    const debt = {
      amount: 0,
      description: '',
      image_id: debts[index].image_id,
    }

    addImageDebt(this.props.token, debt).then(debt =>
      this.props.getDebts(false, debt)
    )
  }

  render() {
    const debts = this.state.debts
    const index = this.state.index
    const assignedTo = this.state.assignedTo
    const showReceipt = this.state.showReceipt

    return (
      <>
        <Modal
          show={this.props.show}
          aria-labelledby="contained-modal-title-vcenter"
          onHide={this.closeModal}
          centered
        >
          <Modal.Header closeButton>
            <div className="container">
              <div className="row d-flex justify-content-center w-100">
                <Button
                  variant="link"
                  className="no-focus"
                  disabled={showReceipt || index === 0}
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
                  disabled={showReceipt || index === debts.length - 1}
                  onClick={this.indexUp}
                >
                  <IconContext.Provider
                    value={{ size: '23px', color: '#343a40' }}
                  >
                    <IoIosArrowForward />
                  </IconContext.Provider>
                </Button>
              </div>
              <div className="row d-flex justify-content-center w-100">
                <Button
                  variant="link"
                  onClick={() => this.setState({ showReceipt: !showReceipt })}
                >
                  {!showReceipt ? 'Show receipt' : 'Show debts'}
                </Button>
              </div>
            </div>
          </Modal.Header>
          <Modal.Body ref={this._modalBody}>
            <Loading show={this.state.loading} />
            {showReceipt ? (
              <img
                src={this.state.imageUrl}
                style={{ width: '466px', height: 'auto', maxHeight: '500px' }}
              />
            ) : (
              <div className="flex-column">
                <div className="row d-flex justify-content-between">
                  <h5
                    style={{
                      lineHeight: '50px',
                      height: '50px',
                      paddingLeft: '5px',
                    }}
                  >
                    Debt
                  </h5>
                  <Button
                    variant="link"
                    className="no-focus"
                    onClick={this.addDebt}
                  >
                    <IconContext.Provider
                      value={{ size: '20px', color: '#343a40' }}
                    >
                      <BsFillPlusCircleFill />
                    </IconContext.Provider>
                  </Button>
                </div>
                <Form>
                  <Form.Group>
                    <Form.Label>Amount</Form.Label>
                    <Form.Control
                      type="number"
                      min={0}
                      placeholder="Enter amount you paid"
                      value={debts[index].amount}
                      onChange={e => this.setAmount(e.target.value)}
                      disabled={debts[index].satisfied}
                      isInvalid={this.state.amountIsInvalid}
                    />
                    <Form.Control.Feedback type="invalid">
                      Amount must be possitive
                    </Form.Control.Feedback>
                  </Form.Group>
                  <Form.Group>
                    <Form.Label>Description</Form.Label>
                    <Form.Control
                      type="text"
                      placeholder="Enter description"
                      value={debts[index].description}
                      onChange={e => this.setDescription(e.target.value)}
                      disabled={debts[index].satisfied}
                      isInvalid={this.state.descriptionIsInvalid}
                    />
                    <Form.Control.Feedback type="invalid">
                      {debts[index].description.length === 0
                        ? 'Description cannot be empty'
                        : 'Lenght cannot exceed 35 characters'}
                    </Form.Control.Feedback>
                  </Form.Group>
                  <Form.Group>
                    <Form.Label>Assigned to</Form.Label>
                    <Form.Control
                      as="select"
                      value={assignedTo[index]}
                      onChange={e => this.setAssignedTo(e.target.value)}
                      disabled={debts[index].satisfied}
                    >
                      <option disabled value={0} key={0}>
                        Not assigned
                      </option>
                      {this.props.users.map(u => (
                        <option
                          disabled={u.id === this.props.user_id}
                          value={u.id}
                          key={u.id}
                        >
                          {u.name}
                        </option>
                      ))}
                    </Form.Control>
                  </Form.Group>
                </Form>
              </div>
            )}
          </Modal.Body>
          <Modal.Footer>
            <Button variant="outline-secondary" onClick={this.closeModal}>
              Close
            </Button>
            <Button
              variant="outline-primary"
              disabled={showReceipt}
              onClick={this.saveDebt}
            >
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

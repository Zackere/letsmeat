import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Dropdown, OverlayTrigger, Tooltip } from 'react-bootstrap'

import { getImageDebts } from '../../../../services/eventService'
import { deleteImageDebt } from '../../../../services/imageService'
import { addDebt, approveDebt } from '../../../../services/debtService'
import { getGroup } from '../../../../services/groupsService'
import { rejectDebt } from '../../../../services/debtService'

import EditModal from './modal/EditDebts'
import { Toggle } from '../../../../common/toggle/Toggle'
import Loading from '../../../../common/loading/Loading'
import { success, error } from '../../../../common/toasts/toasts'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../../../common/styles/styles.css'

class Debts extends Component {
  constructor(props) {
    super(props)

    this.state = {
      loading: false,
      modalOpened: false,
      debts: [],
      selectedDebt: undefined,
      users: [],
      imageId: undefined,
      updateModal: true
    }
  }

  componentDidMount() {
    this.setState({ loading: true })
    getGroup(this.props.token, this.props.groupId).then(group => {
      this.setState({ users: group.users })
      this.getDebts()
    })
  }

  componentDidUpdate(prevProps) {
    if (this.props.update !== prevProps.update) {
      this.getDebts(true)
    }
  }

  removeMe = users => {
    const result = [...users]
    const index = result.findIndex(u => u.id === this.props.user.id)

    return result.splice(index, 1)
  }

  getDebts = (openModal, debt) => {
    this.setState({ loading: true })
    getImageDebts(this.props.token, this.props.eventId)
      .then(res => {
        this.setState({
          loading: false,
          debts: res.image_debts,
          selectedDebt: debt || res.image_debts[0],
          updateModal: !this.state.updateModal
        })
      })
      .then(() => {
        if (openModal) this.setState({ modalOpened: true })
      })
  }

  openModal = debt =>
    debt.image_uploaded_by_id === this.props.user.id &&
    this.setState({
      selectedDebt: debt,
      modalOpened: true,
      updateModal: !this.state.updateModal
    })

  closeModal = () => {
    this.setState({ modalOpened: false })
    this.getDebts()
  }

  debtDelete = debt => {
    this.setState({ loading: true })
    if (debt.pending_debt) {
      rejectDebt(this.props.token, debt.pending_debt.id).then(() =>
        this.deleteImageDebt(debt)
      )
    } else {
      this.deleteImageDebt(debt)
    }
  }

  deleteImageDebt = debt => {
    deleteImageDebt(this.props.token, debt.id).then(res => {
      if (res.ok) {
        success('Debt deleted successfully', this.props.toastManager)
      } else {
        error('Failed to delete debt', this.props.toastManager)
      }
      this.getDebts()
    })
  }
  sameImageDebts = () => {
    const debts = []
    const id = this.state.selectedDebt.image_id

    for (const debt of this.state.debts) {
      if (debt.image_id === id) debts.push(debt)
    }

    return debts
  }

  getTooltipText = debt => {
    if (!debt.pending_debt) return 'Unassigned'

    const user = this.state.users.find(u => debt.pending_debt.from_id === u.id)

    if (debt.satisfied) return user.name + ' is assigned to this'

    return user.name + ' must confirm it is thier'
  }

  assignToMe = d => {
    this.setState({ loading: true })

    if (d.pending_debt) {
      approveDebt(this.props.token, d.pending_debt.id).then(() =>
        this.getDebts(false)
      )
    } else {
      const debt = {
        group_id: this.props.groupId,
        event_id: this.props.eventId,
        from_id: this.props.user.id,
        to_id: d.image_uploaded_by_id,
        image_debt_id: d.id,
        debt_type: 0,
      }

      addDebt(this.props.token, debt).then(() => this.getDebts(false))
    }
  }

  render() {
    const debts = this.state.debts

    if (!this.state.selectedDebt) return <div />
    return (
      <>
        <Loading show={this.state.loading} />
        <EditModal
          show={this.state.modalOpened && debts.length}
          users={this.state.users}
          debts={this.sameImageDebts()}
          closeModal={this.closeModal}
          groupId={this.props.groupId}
          eventId={this.props.eventId}
          imageId={this.state.imageId}
          getDebts={this.getDebts}
          deleteDebt={this.debtDelete}
          selected={this.state.selectedDebt}
          update={this.state.updateModal}
        />
        <ListGroup className="list-scroll" style={{ height: '40vh' }}>
          {debts &&
            debts.map(debt => (
              <ListGroup.Item
                key={debt.id}
                style={{ borderLeft: debt.satisfied ? '5px solid green' : '' }}
                action
              >
                <div className="row align-items-center">
                  <OverlayTrigger
                    key={debt.id}
                    overlay={
                      <Tooltip id={`tooltip-${debt.id}`}>
                        {this.getTooltipText(debt)}
                      </Tooltip>
                    }
                  >
                    <div
                      className="d-flex flex-column col-9 mx-auto"
                      onClick={() => this.openModal(debt)}
                    >
                      <span>{debt.amount / 100 + 'z??'}</span>
                      <small>{debt.description}</small>
                    </div>
                  </OverlayTrigger>
                  <div className="col-3">
                    <Dropdown>
                      <Dropdown.Toggle as={Toggle} />
                      {debt.image_uploaded_by_id !== this.props.user.id ? (
                        <Dropdown.Menu size="sm" title="">
                          <Dropdown.Item
                            onClick={() => this.assignToMe(debt)}
                            disabled={debt.satisfied}
                          >
                            Assign to me
                          </Dropdown.Item>
                        </Dropdown.Menu>
                      ) : (
                        <Dropdown.Menu size="sm" title="">
                          <Dropdown.Item
                            onClick={() => this.openModal(debt)}
                            disabled={debt.satisfied}
                          >
                            Show
                          </Dropdown.Item>
                          <Dropdown.Item
                            onClick={() => this.debtDelete(debt)}
                            disabled={debt.satisfied}
                          >
                            Delete
                          </Dropdown.Item>
                        </Dropdown.Menu>
                      )}
                    </Dropdown>
                  </div>
                </div>
              </ListGroup.Item>
            ))}
        </ListGroup>
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
  user: state.user,
})

export default withToastManager(
  withRouter(connect(mapStateToProps, null)(Debts))
)

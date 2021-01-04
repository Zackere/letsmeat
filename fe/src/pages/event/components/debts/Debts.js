import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Dropdown, OverlayTrigger, Tooltip } from 'react-bootstrap'

import { getImageDebts } from '../../../../services/eventService'
import { getGroup } from '../../../../services/groupsService'

import EditModal from './modal/EditDebts'
import { Toggle } from '../../../../common/toggle/Toggle'
import Loading from '../../../../common/loading/Loading'

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
    }
  }

  componentDidMount() {
    this.setState({ loading: true })
    getGroup(this.props.token, this.props.groupId).then(group => {
      this.setState({ users: group.users })
      this.getDebts(true)
    })
  }

  componentDidUpdate(prevProps) {
    if (this.props.update !== prevProps.update) {
      this.getDebts(true)
    }
  }

  getDebts = openModal => {
    this.setState({ loading: true })
    getImageDebts(this.props.token, this.props.eventId)
      .then(res => {
        this.setState({
          loading: false,
          debts: res.image_debts,
        })
        return res.image_debts[0]
      })
      .then(debt => {
        console.log('open modal', openModal)
        if (openModal) this.setState({ selectedDebt: debt, modalOpened: true })
      })
  }

  openModal = debt => this.setState({ selectedDebt: debt, modalOpened: true })
  closeModal = () => {
    this.setState({ modalOpened: false })
    this.getDebts()
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
    console.log(debt)
    if (!debt.pending_debt) return 'Unassigned'

    const user = this.state.users.find(u => debt.pending_debt.from_id === u.id)

    if (debt.satisfied) return user.name + ' is assigned to this'

    return user.name + ' must confirm it is thier'
  }

  render() {
    const debts = this.state.debts

    if (!this.state.selectedDebt) return <div />
    return (
      <>
        <Loading show={this.state.loading} />
        <EditModal
          show={this.state.modalOpened}
          users={this.state.users}
          debts={this.sameImageDebts()}
          closeModal={this.closeModal}
          groupId={this.props.groupId}
          eventId={this.props.eventId}
          getDebts={this.getDebts}
          selected={this.state.selectedDebt}
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
                      <span>{debt.amount / 100 + 'zł'}</span>
                      <small>{debt.description}</small>
                    </div>
                  </OverlayTrigger>
                  <div className="col-3">
                    <Dropdown>
                      <Dropdown.Toggle as={Toggle} />
                      {debt.image_uploaded_by_id !== this.props.user.id ? (
                        <Dropdown.Menu size="sm" title="">
                          <Dropdown.Item disabled={debt.satisfied}>
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
                          <Dropdown.Item disabled={debt.satisfied}>
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

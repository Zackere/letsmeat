import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Dropdown } from 'react-bootstrap'

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
      if (debt.image_id === id && !debt.satisfied) debts.push(debt)
    }

    return debts
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
              <ListGroup.Item key={debt.id} action>
                <div
                  onClick={() => this.openModal(debt)}
                  className="row align-items-center"
                >
                  <div className="d-flex flex-column col-9 mx-auto">
                    <span>{debt.amount / 100 + 'zł'}</span>
                    <small>{debt.description}</small>
                  </div>
                  <div className="col-3">
                    {debt.satisfied ? (
                      <div>Debt already assigned</div>
                    ) : (
                      <Dropdown>
                        <Dropdown.Toggle as={Toggle} />
                        {debt.image_uploaded_by_id !== this.props.user.id ? (
                          <Dropdown.Menu size="sm" title="">
                            <Dropdown.Item>Assign to me</Dropdown.Item>
                          </Dropdown.Menu>
                        ) : (
                          <Dropdown.Menu size="sm" title="">
                            <Dropdown.Item onClick={() => this.openModal(debt)}>
                              Show
                            </Dropdown.Item>
                            <Dropdown.Item>Delete</Dropdown.Item>
                          </Dropdown.Menu>
                        )}
                      </Dropdown>
                    )}
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

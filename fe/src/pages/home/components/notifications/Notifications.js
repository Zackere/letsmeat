import React, { Component } from 'react'
import { connect } from 'react-redux'
import { ListGroup, Button } from 'react-bootstrap'

import Invitation from './components/Invitation'
import Debt from './components/Debt'
import SetPreferences from './modals/SetPreferences'

import { getInvitations } from '../../../../services/invitationService'
import { getPendingDebts } from '../../../../services/debtService'

class Notifications extends Component {
  constructor(props) {
    super(props)
    this.state = {
      notifications: [],
      preferencesModalOpened: false,
    }
  }

  componentDidMount() {
    getInvitations(this.props.token).then(invitations => {
      getPendingDebts(this.props.token).then(res => {
        const notifications = this.sortNotifications(
          invitations,
          res.pending_debts
        )
        this.setState({ notifications })
      })
    })
  }

  sortNotifications = (inv, deb) => {
    const notifications = [...inv, ...deb]

    notifications.sort(
      (a, b) =>
        new Date(b.timestamp || b.sent).valueOf() -
        new Date(a.timestamp || a.sent).valueOf()
    )

    return notifications
  }

  toUpper(str) {
    return str
      .toLowerCase()
      .split(' ')
      .map(word => word[0].toUpperCase() + word.substr(1))
      .join(' ')
  }

  openPreferencesModal = () => this.setState({ preferencesModalOpened: true })

  closePreferencesModal = () => this.setState({ preferencesModalOpened: false })

  removeInvitation = group_id => {
    const notifications = [...this.state.notifications]

    const index = notifications.findIndex(
      n => !n.id && n.group_id === group_id
    )

    notifications.splice(index, 1)

    this.setState({ notifications })
    this.props.reloadGroups()
  }

  removeTransaction = id => {
    const notifications = [...this.state.notifications]

    const index = notifications.findIndex(
      n => n.id && n.id === id
    )

    notifications.splice(index, 1)

    this.setState({ notifications })
  }

  render() {

    return (
      <>
        <SetPreferences
          show={this.state.preferencesModalOpened}
          closeModal={this.closePreferencesModal}
        />
        <div className="w-100 h-100" style={{ backgroundColor: 'white' }}>
          <div className="h-25 py-5 flex-column">
            <div className="d-flex justify-content-center mb-3">
              <img
                src={this.props.user.picture_url}
                width="50px"
                height="50px"
                className="rounded-circle"
                alt=""
              />
            </div>
            <div className="d-flex justify-content-center">
              <h6>{this.toUpper(this.props.user.name)}</h6>
            </div>
            <div className="d-flex justify-content-center mb-2">
              <span>{this.props.user.email}</span>
            </div>
            <div className="d-flex justify-content-center">
              <Button
                variant="link"
                size="sm"
                onClick={this.openPreferencesModal}
              >
                Set preferences
              </Button>
            </div>
          </div>
          <div className="flex-column h-75 p-5 mx-0 border-top">
            <h5 className="mb-4">Notifications</h5>
            <ListGroup
              className="list-scroll d-flex"
              style={{ maxHeight: '55vh', height: '100%' }}
            >
              {this.state.notifications.map(n => (
                <ListGroup.Item className="rounded-0" key={n.group_id}>
                  {n.amount ? (
                    <Debt
                      from_id={n.to_id}
                      group_id={n.group_id}
                      debt_id={n.id}
                      amount={n.amount / 100}
                      type = {n.debt_type}
                      description={n.description}
                      removeTransaction={this.removeTransaction}
                    />
                  ) : (
                    <Invitation
                      from_id={n.from_id}
                      group_id={n.group_id}
                      removeInvitation={this.removeInvitation}
                    />
                  )}
                </ListGroup.Item>
              ))}
              {this.state.notifications.length > 0 ? (
                <div />
              ) : (
                <span className="align-self-center">
                  You have no notifications
                </span>
              )}
            </ListGroup>
          </div>
        </div>
      </>
    )
  }
}

const mapStateToProps = state => ({
  user: state.user,
  token: state.token,
})
export default connect(mapStateToProps, null)(Notifications)

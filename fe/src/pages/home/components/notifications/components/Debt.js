import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Button } from 'react-bootstrap'
import { withToastManager } from 'react-toast-notifications'

import Loading from '../../../../../common/loading/Loading'
import { success, error } from '../../../../../common/toasts/toasts'

import { getUsers } from '../../../../../services/userService'
import { getGroup } from '../../../../../services/groupsService'
import { approveDebt, rejectDebt } from '../../../../../services/debtService'

class Debt extends Component {
  constructor(props) {
    super(props)
    this.state = {
      groupName: '',
      userName: '',
      loading: false,
    }
  }

  componentDidMount() {
    this.setState({ loading: true })

    getUsers(this.props.token, [this.props.from_id])
      .then(users => this.setState({ userName: users[0].name }))
      .then(() => getGroup(this.props.token, this.props.group_id))
      .then(group => this.setState({ groupName: group.name }))
      .then(() => this.setState({ loading: false }))
  }

  acceptTransaction = () => {
    this.setState({ loading: true })
    approveDebt(this.props.token, this.props.debt_id)
      .then(() => {
        this.props.removeTransaction(this.props.debt_id)
        this.setState({ loading: false })
        success('Transaction accepted successfully', this.props.toastManager)
      })
      .catch(() => {
        this.setState({ loading: false })
        error('Failed to accept transaction', this.props.toastManager)
      })
  }

  rejectTransaction = () => {
    this.setState({ loading: true })
    rejectDebt(this.props.token, this.props.debt_id)
      .then(() => {
        this.props.removeTransaction(this.props.debt_id)
        this.setState({ loading: false })
        success('Transaction rejected successfully', this.props.toastManager)
      })
      .catch(() => {
        this.setState({ loading: false })
        error('Failed to reject transaction', this.props.toastManager)
      })
  }

  render() {
    return (
      <div className="flex-column w-100 h-100">
        <Loading show={this.state.loading} />
        <p>
          <b> {this.state.userName + ' '}</b>
          <small>from group</small>
          <b> {' ' + this.state.groupName + ' '}</b>
          <small>{this.props.type ? 'sent you ' : 'want you to pay '}</small>
          <br/>
          <b> {this.props.amount + ' zł'}</b>
          <br />
          <small>{this.props.description}</small>
        </p>
        <div className="d-flex float-right">
          <Button
            variant="outline-danger"
            className="mr-2 border-0"
            onClick={this.rejectTransaction}
          >
            Reject
          </Button>
          <Button
            variant="outline-primary"
            className="ml-2 border-0"
            onClick={this.acceptTransaction}
          >
            Accept
          </Button>
        </div>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})
export default withToastManager(connect(mapStateToProps, null)(Debt))

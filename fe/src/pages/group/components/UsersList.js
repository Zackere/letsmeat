import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Dropdown } from 'react-bootstrap'

import { Toggle } from '../../../common/toggle/Toggle'
import ShowBalance from '../modals/ShowBalance'
import Loading from '../../../common/loading/Loading'
import { debtsInfo, addDebt } from '../../../services/debtService'
import { success, error } from '../../../common/toasts/toasts'

import 'bootstrap/dist/css/bootstrap.min.css'

import '../../../common/styles/styles.css'

class UsersList extends Component {
  constructor(props) {
    super(props)

    this.state = {
      balanceModalOpened: false,
      loading: false,
      activeUser: {},
      debts: {},
    }
  }

  componentDidMount() {
    this.setState({ loading: true })
    debtsInfo(this.props.token, this.props.groupId).then(res =>
      this.setState({ debts: res.debts, loading: false })
    )
  }

  getDebt = () => {
    const me = this.props.me
    const user = this.state.activeUser

    if (!me || !user) return 0

    const from = me.id > user.id ? me.id : user.id
    const to = me.id < user.id ? me.id : user.id

    console.log(from)
    console.log(to)

    if (!this.state.debts[from] || !this.state.debts[from][to]) return 0

    return this.state.debts[from][to]
  }

  sendMoney = (amount, description) => {
    const from_id = this.state.activeUser.id
    const to_id = this.props.me.id

    const body = {
      group_id: this.props.groupId,
      from_id,
      to_id,
      amount: parseInt(amount),
      description,
      debt_type: 1
    }

    this.setState({ loading: true })
    addDebt(this.props.token, body)
      .then(() => {
        this.setState({ loading: false })
        success(
          'Your transaction is pending, ' +
            this.state.activeUser.name +
            ' must confirm it',
          this.props.toastManager
        )
      })
      .catch(e => {
        this.setState({ loading: false })
        error('Failed to process your transaction', this.props.toastManager)
      })
  }

  closeBalanceModal = () => this.setState({ balanceModalOpened: false })

  render() {
    const users = this.props.users
    const me = this.props.me
    const debt = this.getDebt()/100

    return (
      <>
        <Loading show={this.state.loading} />
        <ShowBalance
          show={this.state.balanceModalOpened}
          user={this.state.activeUser}
          me={this.props.me}
          debt={debt}
          closeModal={this.closeBalanceModal}
          sendMoney={this.sendMoney}
        />
        <ListGroup className="list-scroll" style={{ height: '50vh' }}>
          {users &&
            users.map(user => (
              <ListGroup.Item key={user.id} action>
                <div className="row align-items-center rounded-0 border-left-0">
                  <div
                    className="col-9 mx-auto"
                    onClick={() =>
                      this.setState({
                        balanceModalOpened: true,
                        activeUser: user,
                      })
                    }
                  >
                    <img
                      src={user.picture_url}
                      width="42px"
                      className="mr-3 rounded-circle"
                      alt=""
                    />
                    <span>{user.name}</span>
                  </div>
                  <div className="col-3">
                    <Dropdown>
                      <Dropdown.Toggle as={Toggle} />
                      <Dropdown.Menu size="sm" title="">
                        <Dropdown.Item
                          disabled={me.id === user.id}
                          onClick={() =>
                            this.setState({
                              balanceModalOpened: true,
                              activeUser: user,
                            })
                          }
                        >
                          Show balance
                        </Dropdown.Item>
                      </Dropdown.Menu>
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
  me: state.user,
})
export default withToastManager(connect(mapStateToProps, null)(UsersList))

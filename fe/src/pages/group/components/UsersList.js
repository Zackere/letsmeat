import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Dropdown } from 'react-bootstrap'

import { Toggle } from '../../../common/toggle/Toggle'
import ShowBalance from '../modals/ShowBalance'
import { debtsInfo } from '../../../services/debtService'

import 'bootstrap/dist/css/bootstrap.min.css'

import '../../../common/styles/styles.css'

class UsersList extends Component {
  constructor(props) {
    super(props)

    this.state = {
      balanceModalOpened: false,
      activeUser: {},
      debts: {},
    }
  }

  componentDidMount() {
    debtsInfo(this.props.token, this.props.groupId).then(debts =>
      this.setState({ debts })
    )
  }
  deleteUser = name => console.log('lets delete ' + name)

  closeBalanceModal = () => this.setState({ balanceModalOpened: false })
  render() {
    const users = this.props.users
    return (
      <>
        <ShowBalance
          show={this.state.balanceModalOpened}
          user={this.state.activeUser}
          me={this.props.me}
          debt={0}
          closeModal={this.closeBalanceModal}
        />
        <ListGroup className="list-scroll">
          {users &&
            users.map(user => (
              <ListGroup.Item key={user.id} action>
                <div className="row align-items-center">
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
                          onClick={() =>
                            this.setState({
                              balanceModalOpened: true,
                              activeUser: user,
                            })
                          }
                        >
                          Show balance
                        </Dropdown.Item>
                        <Dropdown.Item onClick={event => console.log(event)}>
                          Delete
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

import React, { Component } from 'react'
import { Form, ListGroup, Button, InputGroup } from 'react-bootstrap'
import { IconContext } from 'react-icons'
import { TiDelete } from 'react-icons/ti'
import { BiSearch } from 'react-icons/bi'
import { connect } from 'react-redux'


import Loading from '../../../common/loading/Loading'
import { searchUsers } from '../../../services/userService'
import '../../../common/styles/styles.css'

class UserSearch extends Component {
  constructor(props) {
    super(props)

    this.state = {
      users: [],
      selected: [],
      loading: false,
    }
  }

  filterArray = (users, selected) => {
    const modifiedUsers = [...users]
    let index = -1

    for (let user of selected) {
      if ((index = users.findIndex(u => u.id === user.id)) !== -1) {
        modifiedUsers.splice(index, 1)
      }
    }
    return modifiedUsers
  }

  getUsers = event => {
    const name = event.target.value.trim()

    if (name) {
      this.setState({ loading: true })
      searchUsers(this.props.token, name).then(users => {
        const filteredUsers = this.filterArray(users, this.state.selected)

        this.setState({ users: filteredUsers })
        this.setState({ loading: false })
      })
    } else this.setState({ users: [] })
  }

  selectUser = user => {
    const selected = [user, ...this.state.selected]
    const users = this.filterArray(this.state.users, selected)
    
    this.props.setSelected(selected)
    this.setState({ selected })
    this.setState({ users })
  }

  deleteSelection = id => {
    const selected = [...this.state.selected]

    const index = selected.findIndex(user => user.id === id)
    selected.splice(index, 1)

    this.props.setSelected(selected)
    this.setState({ selected })
  }

  render() {
    return (
      <div>
        <Loading show={this.state.loading} />
        <Form>
          <Form.Group>
          <Form.Label>User name</Form.Label>
          <InputGroup>
                <InputGroup.Prepend>
                  <InputGroup.Text>
                    <BiSearch />
                  </InputGroup.Text>
                </InputGroup.Prepend>
            <Form.Control
              type="text"
              placeholder="Invite new member"
              onChange={this.getUsers}
            />
            </InputGroup>
          </Form.Group>
        </Form>
        <div className="row mx-0 mb-1 px-2">
          {this.state.selected.map(user => (
            <div key={user.id}>
              <img
                src={user.picture_url}
                width="36px"
                className="rounded-circle"
                alt=''
              />
              <Button
                style={{
                  position: 'relative',
                  top: '-14px',
                  right: '24px',
                }}
                className="no-focus"
                variant="link"
                onClick={() => this.deleteSelection(user.id)}
              >
                <IconContext.Provider value={{ size: '20px' }}>
                  <TiDelete />
                </IconContext.Provider>
              </Button>
            </div>
          ))}
        </div>
        <ListGroup className="list-scroll">
          {this.state.users.map(user => (
            <ListGroup.Item
              key={user.id}
              action
              onClick={() => this.selectUser(user)}
            >
              <div className="row">
                <div className="col-9">
                  <img
                    src={user.picture_url}
                    width="36px"
                    className="mr-3 rounded-circle"
                    alt=''
                  />
                  {user.name}
                </div>
              </div>
            </ListGroup.Item>
          ))}
        </ListGroup>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default connect(mapStateToProps, null)(UserSearch)

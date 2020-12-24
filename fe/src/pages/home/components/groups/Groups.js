import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Container, Dropdown } from 'react-bootstrap'

import { Toggle } from '../../../../common/toggle/Toggle'
import ControlPanel from '../../../../common/control panel/ControlPanel'
import Loading from '../../../../common/loading/Loading'
import {
  getGroups,
  addGroup,
  deleteGroup,
  leaveGroup,
} from '../../../../services/groupsService'
import { success, error } from '../../../../common/toasts/toasts'
import AddGroup from './modals/AddGroup'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../../../common/styles/styles.css'

class Groups extends Component {
  state = {
    groups: [],
    all: [],
    addModalOpened: false,
    loading: false,
  }

  componentDidMount() {
    this.getGroups()
  }

  componentDidUpdate(previousProps) {
    previousProps.reload != this.props.reload && this.getGroups()
  }

  getGroups = () => {
    this.setState({ loading: true })
    getGroups(this.props.token).then(groups => {
      this.setState({ groups, all: groups, loading: false })
    })
  }

  goToGroupPage = id => this.props.history.push('/group:' + id)

  addGroup = name => {
    this.setState({ loading: true })
    addGroup(this.props.token, name)
      .then(group => {
        const groups = [group, ...this.state.groups]
        const all = [group, ...this.state.all]

        this.setState({ all, groups, loading: false })
        success(
          'Group ' + group.name + ' added successfully',
          this.props.toastManager
        )
      })
      .catch(e => {
        error('Failed to add group ' + name, this.props.toastManager)
        this.setState({ loading: false })
      })
  }

  buttonAction = () => this.setState({ addModalOpened: true })

  searchAction = event => {
    const groups = this.state.all.filter(g =>
      g.name.startsWith(event.target.value)
    )

    this.setState({ groups })
  }

  closeModal = () => this.setState({ addModalOpened: false })

  modifyGroups = id => {
    const groups = [...this.state.groups]
    const all = [...this.state.all]

    let index = groups.findIndex(group => group.id === id)
    groups.splice(index, 1)

    index = all.findIndex(group => group.id === id)
    all.splice(index, 1)

    this.setState({ all, groups })
  }

  deleteGroup = group => {
    this.setState({ loading: true })
    deleteGroup(this.props.token, group.id).then(res => {
      if (res.ok) {
        this.modifyGroups(group.id)
        success(
          'Group ' + group.name + ' deleted successfully',
          this.props.toastManager
        )
      } else {
        error('Failed to delete group ' + group.name, this.props.toastManager)
      }

      this.setState({ loading: false })
    })
  }

  leaveGroup = group => {
    this.setState({ loading: true })
    leaveGroup(this.props.token, group.id).then(res => {
      if (res.ok) {
        this.modifyGroups(group.id)
        success(
          'Group ' + group.name + ' left successfully',
          this.props.toastManager
        )
      } else {
        error('Failed to leave group ' + group.name, this.props.toastManager)
      }

      this.setState({ loading: false })
    })
  }

  render() {
    return (
      <Container fluid={true}>
        <Loading show={this.state.loading} />
        <div className="mt-5">
          <h4>Groups</h4>
        </div>
        <AddGroup
          show={this.state.addModalOpened}
          onClose={this.closeModal}
          addGroup={this.addGroup}
        />
        <ControlPanel
          buttonName="Add"
          buttonAction={this.buttonAction}
          searchName="Find group"
          searchAction={this.searchAction}
        />
        <ListGroup className="list-scroll">
          {this.state.groups.map(group => (
            <ListGroup.Item key={group.id} action>
              <div className="row">
                <div
                  className="col-11 my-auto"
                  onClick={() => this.goToGroupPage(group.id)}
                >
                  {group.name}
                </div>
                <div className="col-1">
                  <Dropdown>
                    <Dropdown.Toggle as={Toggle} />
                    <Dropdown.Menu size="sm" title="">
                      <Dropdown.Item
                        onClick={() => this.goToGroupPage(group.id)}
                      >
                        Show
                      </Dropdown.Item>
                      {this.props.user.id === group.owner_id ? (
                        <Dropdown.Item onClick={() => this.deleteGroup(group)}>
                          Delete
                        </Dropdown.Item>
                      ) : (
                        <Dropdown.Item onClick={() => this.leaveGroup(group)}>
                          Leave
                        </Dropdown.Item>
                      )}
                    </Dropdown.Menu>
                  </Dropdown>
                </div>
              </div>
            </ListGroup.Item>
          ))}
        </ListGroup>
      </Container>
    )
  }
}

const mapStateToProps = state => ({
  user: state.user,
  token: state.token,
})

export default withToastManager(
  withRouter(connect(mapStateToProps, null)(Groups))
)

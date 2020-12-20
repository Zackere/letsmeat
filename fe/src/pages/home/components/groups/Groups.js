import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { ListGroup, Container, Dropdown } from 'react-bootstrap'

import 'bootstrap/dist/css/bootstrap.min.css'

import { Toggle } from '../../../../common/toggle/Toggle'
import ControlPanel from '../../../../common/control panel/ControlPanel'
import Loading from '../../../../common/loading/Loading'
import { getGroups, addGroup, deleteGroup } from '../../../../services/groupsService'
import AddGroup from './modals/AddGroup'

import '../../../../common/styles/styles.css'

class Groups extends Component {
  state = {
    groups: [],
    all: [],
    addModalOpened: false,
    loading: false,
  }

  componentDidMount() {
    getGroups(this.props.token).then(groups => {
      this.setState({ groups: groups, all: groups })
    })
  }

  goToGroupPage(id) {
    this.props.history.push('/group:' + id)
  }

  addGroup = name => {
    this.setState({ loading: true })
    addGroup(this.props.token, name).then(group => {
      const modifiedGroups = [group, ...this.state.groups]
      const modifiedAll = [group, ...this.state.all]

      this.setState({ all: modifiedAll })
      this.setState({ groups: modifiedGroups })
      this.setState({ loading: false })
    })
  }

  buttonAction = () => this.setState({ addModalOpened: true })

  searchAction = event => {
    const modifiedGroups = this.state.all.filter(g =>
      g.name.startsWith(event.target.value)
    )

    this.setState({ groups: modifiedGroups })
  }

  closeModal = () => this.setState({ addModalOpened: false })

  deleteGroup = id => {
    this.setState({ loading: true })
    deleteGroup(this.props.token, id).then(() => {
      let modifiedGroups = [...this.state.groups]

      let index = modifiedGroups.findIndex(group => group.id === id)
      modifiedGroups.splice(index, 1)

      let modifiedAll = [...this.state.all]

      index = modifiedAll.findIndex(group => group.id === id)
      modifiedAll.splice(index, 1)

      this.setState({ all: modifiedAll })
      this.setState({ groups: modifiedGroups })
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
                      <Dropdown.Item onClick={() => this.deleteGroup(group.id)}>
                        Delete
                      </Dropdown.Item>
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

export default withRouter(connect(mapStateToProps, null)(Groups))

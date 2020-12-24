import React, { Component } from 'react'
import { Container, Button } from 'react-bootstrap'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'

import ControlPanel from '../../common/control panel/ControlPanel'
import { getGroup } from '../../services/groupsService'
import InviteUser from './modals/InviteUser'
import AddEvent from './modals/AddEvent'
import UsersList from './components/UsersList'
import EventsList from './components/EventsList'

import '../../common/styles/styles.css'

class Group extends Component {
  constructor(props) {
    super(props)
    this.state = {
      id: props.match.params.id.substring(1),
      ownerId: 0,
      group: {},
      users: [],
      allUsers: [],
      events: [],
      allEvents: [],
      invModalOpen: false,
      eventModalOpen: false,
    }
  }

  componentDidMount() {
    getGroup(this.props.token, this.state.id).then(group => {
      this.setState({
        allUsers: group.users,
        users: group.users,
        allEvents: group.events,
        events: group.events,
        ownerId: group.owner_id,
        group,
      })
    })
  }

  userSearchAction = e => {
    const users = this.state.allUsers.filter(u =>
      u.name.startsWith(e.target.value)
    )

    this.setState({ users })
  }

  eventSearchAction = e => {
    const events = this.state.allEvents.filter(event =>
      event.name.startsWith(e.target.value)
    )

    this.setState({ events })
  }

  addEvent = event => {
    const allEvents = [event, ...this.state.allEvents]
    const events = [event, ...this.state.events]

    this.setState({ allEvents, events })
  }

  deleteEvent = id => {
    const events = [...this.state.events]
    const allEvents = [...this.state.allEvents]

    let index = events.findIndex(event => event.id === id)
    events.splice(index, 1)

    index = allEvents.findIndex(event => event.id === id)
    allEvents.splice(index, 1)

    this.setState({ allEvents, events })
  }

  openInvitationModal = () => this.setState({ invModalOpen: true })

  eventButtonAction = () => this.setState({ eventModalOpen: true })

  closeInvModal = () => this.setState({ invModalOpen: false })

  closeEventModal = () => this.setState({ eventModalOpen: false })

  render() {
    return (
      <>
        <InviteUser
          show={this.state.invModalOpen}
          closeModal={this.closeInvModal}
          groupId={this.state.group.id}
        />
        <AddEvent
          show={this.state.eventModalOpen}
          addEvent={this.addEvent}
          closeModal={this.closeEventModal}
          groupId={this.state.group.id}
        />
        <div className="row mx-0" style={{ height: '94vh'}}>
          <div
            className="col-3 pt-3 px-0 border border-right"
            style={{ background: 'white' }}
          >
            <div className="h-25 py-5 flex-column border-bottom">
              <div className="d-flex justify-content-center h-75 align-items-center">
                <h4>{this.state.group.name}</h4>
              </div>
              <div className="d-flex justify-content-center">
                <Button
                  variant="link"
                  size="sm"
                  onClick={this.openInvitationModal}
                >
                  Add new member
                </Button>
              </div>
            </div>
            <div className="flex-column h-75 p-5 mx-0 border-top">
              <h5 className="mb-4">Members</h5>
              <UsersList
                users={this.state.users}
                groupId={this.state.id}
                ownerId={this.state.owner_id}
              />
            </div>
          </div>
          <div className="col-9 p-5">
            <h4>Events</h4>
            <ControlPanel
              buttonAction={this.eventButtonAction}
              searchName="Find event"
              searchAction={this.eventSearchAction}
            />
            <EventsList
              events={this.state.events}
              deleteEvent={this.deleteEvent}
            />
          </div>
        </div>
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(Group))

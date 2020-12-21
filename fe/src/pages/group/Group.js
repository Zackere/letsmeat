import React, { Component } from 'react'
import { Container } from 'react-bootstrap'
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
        group,
      })
    })
  }

  userSearchAction = e => {
    const modifiedUsers = this.state.allUsers.filter(u =>
      u.name.startsWith(e.target.value)
    )

    this.setState({ users: modifiedUsers })
  }

  eventSearchAction = e => {
    const modifiedEvents = this.state.allEvents.filter(event =>
      event.name.startsWith(e.target.value)
    )

    this.setState({ events: modifiedEvents })
  }

  addEvent = event => {
    const allEvents = [event, ...this.state.allEvents]
    const events = [event, ...this.state.events]

    this.setState({ allEvents, events })
  }
  userButtonAction = () => this.setState({ invModalOpen: true })

  eventButtonAction = () => this.setState({ eventModalOpen: true })

  closeInvModal = () => this.setState({ invModalOpen: false })

  closeEventModal = () => this.setState({ eventModalOpen: false })

  render() {
    return (
      <Container fluid={true}>
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
        <div className="row mt-2">
          <div className="col-4">
            <h2>{this.state.group.name}</h2>
          </div>
        </div>
        <div className="row" style={{ height: '89vh' }}>
          <div className="col-4 border-dark border-top border-right py-2">
            <h4>Members</h4>
            <ControlPanel
              buttonAction={this.userButtonAction}
              searchName="Find user"
              searchAction={this.userSearchAction}
            />
            <UsersList users={this.state.users} groupId={this.state.id} />
          </div>
          <div className="col-8 border-dark border-top py-2">
            <h4>Events</h4>
            <ControlPanel
              buttonAction={this.eventButtonAction}
              searchName="Find event"
              searchAction={this.eventSearchAction}
            />
            <EventsList events={this.state.events} />
          </div>
        </div>
      </Container>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(Group))

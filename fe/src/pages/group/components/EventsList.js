import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup, Dropdown } from 'react-bootstrap'

import { Toggle } from '../../../common/toggle/Toggle'
import Loading from '../../../common/loading/Loading'
import { basicFormat } from '../../../common/date formater/dateFormater'
import { deleteEvent } from '../../../services/eventService'

import { success, error } from '../../../common/toasts/toasts'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../../common/styles/styles.css'

class EventsList extends Component {
  constructor(props) {
    super(props)

    this.state = {
      loading: false,
    }
  }

  goToEventPage = id => this.props.history.push('/event:' + id)

  deleteEvent = event => {
    this.setState({ loading: true })
    deleteEvent(this.props.token, event.id).then(res => {
      if (res.ok) {
        this.props.deleteEvent(event.id)
        success(event.name + ' deleted successfully', this.props.toastManager)
      } else {
        error('Failed to delete event ' + event.name, this.props.toastManager)
      }
      this.setState({ loading: false })
    })
  }

  render() {
    const events = this.props.events
    const user = this.props.user
    return (
      <>
        <Loading show={this.state.loading} />
        <ListGroup className="list-scroll">
          {events &&
            events.map(event => (
              <ListGroup.Item key={event.id} action>
                <div className="row align-items-center">
                  <div
                    className="d-flex flex-column col-9 mx-auto"
                    onClick={() => this.goToEventPage(event.id)}
                  >
                    <span>{event.name}</span>
                    <div className="small-string">
                      {basicFormat(event.deadline)}
                    </div>
                  </div>
                  <div className="col-3">
                    <Dropdown>
                      <Dropdown.Toggle as={Toggle} />
                      <Dropdown.Menu size="sm" title="">
                        <Dropdown.Item onClick={() => this.goToEventPage(event.id)}>
                          Show
                        </Dropdown.Item>{
                        true &&
                        <Dropdown.Item onClick={() => this.deleteEvent(event)}>
                          Delete
                        </Dropdown.Item>
                        }
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
  user: state.user
})

export default withToastManager(withRouter(connect(mapStateToProps, null)(EventsList)))

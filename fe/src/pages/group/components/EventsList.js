import React, { Component } from 'react'
import { ListGroup, Dropdown } from 'react-bootstrap'
import { Toggle } from '../../../common/toggle/Toggle'
import { basicFormat } from '../../../common/date formater/dateFormater'
import 'bootstrap/dist/css/bootstrap.min.css'

import '../../../common/styles/styles.css'

export default class EventsList extends Component {
  deleteEvent = name => console.log('lets delete ' + name)

  render() {
    const events = this.props.events
    return (
      <ListGroup className="list-scroll">
        {events &&
          events.map(event => (
            <ListGroup.Item key={event.id} action>
              <div className="row align-items-center">
                <div
                  className="d-flex flex-column col-9 mx-auto"
                  onClick={() => console.log(event.name)}
                >
                  <span>{event.name}</span>
                  <div className="small-string">{basicFormat(event.deadline)}</div>
                </div>
                <div className="col-3">
                  <Dropdown>
                    <Dropdown.Toggle as={Toggle} />
                    <Dropdown.Menu size="sm" title="">
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
    )
  }
}

import React, { Component } from 'react'
import Groups from './components/groups/Groups'
import { Container } from 'react-bootstrap'
import Notifications from './components/notifications/Notifications'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../common/styles/styles.css'

export default class Home extends Component {
  constructor(props) {
    super(props)
    this.state = {
      reloadGroups: true,
    }
  }
  reloadGroups = () => this.setState({ reloadGroups: !this.state.reloadGroups })

  render() {
    return (
        <div
          className="row mx-0"
          style={{ height: '94vh'}}
        >
          <div className="col-3 px-0">
            <Notifications reloadGroups={this.reloadGroups} />
          </div>
          <div className="col-9 border-left">
            <Groups reload={this.state.reloadGroups} />
          </div>
        </div>
    )
  }
}

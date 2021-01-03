import React, { Component } from 'react'
import Groups from './components/groups/Groups'
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
      <div className="row mx-0" style={{ height: '94vh' }}>
        <div className="col-3 px-0 h-100">
          <Notifications reloadGroups={this.reloadGroups} />
        </div>
        <div
          className="col-9 border-left h-100"
          style={{
            paddingLeft: '270px',
            paddingRight: '270px',
          }}
        >
          <Groups reload={this.state.reloadGroups} />
        </div>
      </div>
    )
  }
}

import React, { Component } from 'react'
import Groups from './components/groups/Groups'
import Notifications from './components/notifications/Notifications'

import 'bootstrap/dist/css/bootstrap.min.css'
import '../../common/styles/styles.css'

export default class Home extends Component {
  render() {
    return (
      <div className="row mx-0" style={{ height: '94vh' }}>
        <div className="col-3 px-0">
          <Notifications />
        </div>
        <div className="col-9 border-left">
          <Groups />
        </div>
      </div>
    )
  }
}

import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'
import { ListGroup } from 'react-bootstrap'

import Loading from '../../../common/loading/Loading'

class Voting extends Component {
  constructor(props) {
    super(props)
    this.state = {
      loading: false,
      locations: props.locations,
    }
  }
  render() {
    const locations = this.state.locations
    return (
      <>
        <Loading show={this.state.loading} />
        <ListGroup className="list-scroll">
          {locations.map(id => (
            <ListGroup.Item key={id} action></ListGroup.Item>
          ))}
        </ListGroup>
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(Voting))

import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'
import { Container } from 'react-bootstrap'

import Loading from '../../common/loading/Loading'
import { getEvent } from '../../services/eventService'
import { getLocations } from '../../services/locationsService'
import Voting from './components/Voting'
import ReceiptReader from './components/ReceiptReader'

class Event extends Component {
  constructor(props) {
    super(props)
    this.state = {
      id: props.match.params.id.substring(1),
      event: {},
      loading: false,
    }
  }

  componentDidMount() {
    this.setState({ loading: true })
    getEvent(this.props.token, this.state.id).then(event =>
      this.setState({ event, loading: false })
    )
  }

  getLocations = event => {
    const body = {
      custom_location_ids: event.candidate_custom_locations,
      google_maps_location_ids: event.candidate_google_maps_locations,
    }
    return getLocations(this.props.token, body)
  }

  render() {
    return (
      <>
        <div className="row mx-0" style={{ height: '94vh'}}>
          <div
            className="col-3 px-0 border border-right"
            style={{ background: 'white' }}
          >
            <div className="h-40 py-5 flex-column border-bottom">
              <div className="d-flex justify-content-center align-items-center">
                <h4>{this.state.event.name}</h4>
              </div>
              <div className="d-flex justify-content-center">
                <ReceiptReader/>
              </div>
            </div>
            <div className="flex-column h-60 p-5 mx-0 border-top">
              <h5 className="mb-4">Debts</h5>
            </div>
          </div>
          <div className="col-9 p-5">
            <h4>Voting</h4>
            <Voting locations={[]}/>
          </div>
        </div>
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(Event))

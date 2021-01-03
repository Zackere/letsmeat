import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'

import Voting from './components/voting/Voting'
import ReceiptReader from './components/receipt reader/ReceiptReader'
import Debts from './components/debts/Debts'
import Loading from '../../common/loading/Loading'

import { getEvent } from '../../services/eventService'
import { processReceipt } from '../../services/imageService'

class Event extends Component {
  constructor(props) {
    super(props)
    this.state = {
      id: props.match.params.id.substring(1),
      event: null,
      loading: false,
      updateDebts: false,
    }
  }

  componentDidMount() {
    this.setState({ loading: true })
    getEvent(this.props.token, this.state.id).then(event =>
      this.setState({ event, loading: false })
    )
  }

  processReceipt = file => {
    this.setState({ loading: true })
    processReceipt(this.props.token, this.state.event.id, file).then(() =>
      this.setState({ updateDebts: !this.state.updateDebts, loading: false })
    )
  }

  render() {
    const event = this.state.event
    return (
      <>
      <Loading show={this.state.loading}/>
        {event && (
          <div className="row mx-0" style={{ height: '94vh' }}>
            <div
              className="col-3 px-0 border border-right h-100"
              style={{ background: 'white' }}
            >
              <div className="h-40 py-5 flex-column border-bottom">
                <div className="d-flex justify-content-center align-items-center">
                  <h4>{event.name}</h4>
                </div>
                <div className="d-flex justify-content-center">
                  <ReceiptReader processReceipt={this.processReceipt} />
                </div>
              </div>
              <div className="flex-column h-60 p-5 mx-0 border-top">
                <h5 className="mb-4">Debts</h5>
                <Debts
                  update={this.state.updateDebts}
                  groupId={this.state.event.group_id}
                  eventId={this.state.event.id}
                />
              </div>
            </div>
            <div className="col-9 p-5 h-100">
              <h4 className="ml-3" style={{ color: 'white' }}>
                Voting
              </h4>
              <Voting event={event} />
            </div>
          </div>
        )}
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(Event))

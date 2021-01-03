import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'

import Loading from '../../../../common/loading/Loading'
import Mask from '../../../../common/mask/Mask'
import Dates from './components/Dates'
import Places from './components/Places'
import RateModal from './modals/RateModal'

import { updateEvent } from '../../../../services/eventService'
import { castVote, getVote } from '../../../../services/votingService'
import {
  createGoogleLocation,
  createCustomLocation,
} from '../../../../services/locationsService'

import { success, error } from '../../../../common/toasts/toasts'

class Voting extends Component {
  constructor(props) {
    super(props)

    this.state = {
      loading: false,
      event: props.event,
      sessiontoken: this.generateToken(),
      locations: [],
      dates: [],
      modalOpened: false,
    }
  }

  componentDidMount() {
    getVote(this.props.token, this.state.event.id).then(res => {
      this.setState({
        locations: res.locations,
        dates: res.times,
        modalOpened: true,
      })
    })
  }

  addDate = date => {
    const event = Object.assign({}, this.state.event)
    if (
      event.candidate_times &&
      event.candidate_times.find(c => date.valueOf() === new Date(c).valueOf())
    )
      error('This date is already added', this.props.toastManager)
    else {
      const candidate_times = [date, ...event.candidate_times]
      event.candidate_times = candidate_times

      updateEvent(this.props.token, event)
        .then(res => {
          if (res.ok) {
            this.setState({ event, dates: [date, ...this.state.dates] })
            success(
              'Event ' + event.name + ' updated successfully',
              this.props.toastManager
            )
          }
        })
        .catch(e =>
          error('Failed to update event ' + event.name, this.props.toastManager)
        )
    }
  }

  addGooglePlace = p => {
    this.setState({ loading: true })
    const event = Object.assign({}, this.state.event)
    if (
      event.candidate_google_maps_locations &&
      event.candidate_google_maps_locations.find(c => c === p.place_id)
    )
      error('This location is already added', this.props.toastManager)
    else {
      const candidate_google_maps_locations = [
        p.place_id,
        ...event.candidate_google_maps_locations,
      ]
      event.candidate_google_maps_locations = candidate_google_maps_locations

      createGoogleLocation(
        this.props.token,
        this.state.sessiontoken,
        p.place_id
      ).then(
        () =>
          updateEvent(this.props.token, event)
            .then(res => {
              if (res.ok) {
                this.setState({
                  event,
                  sessiontoken: this.generateToken(),
                  locations: [
                    {
                      google_maps_location_id: p.place_id,
                      custom_location_id: null,
                    },
                    ...this.state.locations,
                  ],
                  loading: false,
                })
                success(
                  'Event ' + event.name + ' updated successfully',
                  this.props.toastManager
                )
              }
            })
            .catch(e =>
              error(
                'Failed to update event ' + event.name,
                this.props.toastManager
              )
            )
        // this.setState({
        //   sessiontoken: this.generateToken(),
        //   locations: [
        //     {
        //       google_maps_location_id: p.place_id,
        //       custom_location_id: null,
        //     },
        //     ...this.state.locations,
        //   ],
        //   loading: false,
        // })
      )
    }
  }

  addCustomPlace = (name, address) => {
    this.setState({ loading: true })
    const event = Object.assign({}, this.state.event)

    createCustomLocation(
      this.props.token,
      name,
      address,
      this.state.event.group_id
    ).then(place => {
      const candidate_custom_locations = [
        place.id,
        ...event.candidate_custom_locations,
      ]
      event.candidate_custom_locations = candidate_custom_locations

      updateEvent(this.props.token, event)
        .then(res => {
          if (res.ok) {
            this.setState({
              event,
              locations: [
                {
                  google_maps_location_id: null,
                  custom_location_id: place.id,
                },
                ...this.state.locations,
              ],
              loading: false,
            })
            success(
              'Event ' + event.name + ' updated successfully',
              this.props.toastManager
            )
          }
        })
        .catch(e =>
          error('Failed to update event ' + event.name, this.props.toastManager)
        )
    })
  }

  vote = () => {
    castVote(
      this.props.token,
      this.state.dates,
      this.state.locations,
      this.state.event.id
    )
      .then(res => {
        if (res.ok) {
          success('Vote casted successfully', this.props.toastManager)
        }
      })
      .catch(e => error('Failed to cast vote', this.props.toastManager))
  }

  upD = i => {
    if (!i) return
    const dates = [...this.state.dates]

    let d = dates[i]
    dates[i] = dates[i - 1]
    dates[i - 1] = d

    this.setState({ dates })
  }

  downD = i => {
    const dates = [...this.state.dates]

    if (i >= dates.length - 1) return

    let t = dates[i]
    dates[i] = dates[i + 1]
    dates[i + 1] = t

    this.setState({ dates })
  }

  upL = i => {
    if (!i) return

    const locations = [...this.state.locations]

    let d = locations[i]
    locations[i] = locations[i - 1]
    locations[i - 1] = d

    this.setState({ locations })
  }

  downL = i => {
    const locations = [...this.state.locations]

    if (i >= locations.length - 1) return

    let t = locations[i]
    locations[i] = locations[i + 1]
    locations[i + 1] = t

    this.setState({ locations })
  }

  generateToken = () => {
    let result = ''
    for (var i = 0; i < 8; i++) {
      result += Math.random().toString(36).substring(2, 6)
    }

    return result
  }

  closeModal = () => this.setState({ modalOpened: false })

  render() {
    const deadline = new Date(this.props.event.deadline)

    return (
      <>
        <Mask show={deadline < new Date()}/>
        <Loading show={this.state.loading} />
        <RateModal
          show={this.state.modalOpened && deadline < new Date()}
          closeModal={this.closeModal}
          eventId={this.props.event.id}
        />
        <div className="d-flex pt-5">
          <div className="col-4">
            <h5 style={{color: 'white'}}>Dates</h5>
            <Dates
              dates={this.state.dates}
              deadline={deadline}
              addDate={this.addDate}
              up={this.upD}
              down={this.downD}
              vote={this.vote}
            />
          </div>
          <div className="col-8">
            <h5>Places</h5>
            <Places
              locations={this.state.locations}
              groupId={this.state.event.group_id}
              sessiontoken={this.state.sessiontoken}
              addGooglePlace={this.addGooglePlace}
              addCustomPlace={this.addCustomPlace}
              up={this.upL}
              down={this.downL}
              vote={this.vote}
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

export default withToastManager(connect(mapStateToProps, null)(Voting))

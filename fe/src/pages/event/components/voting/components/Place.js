import React, { Component } from 'react'
import { connect } from 'react-redux'
import { OverlayTrigger, Tooltip } from 'react-bootstrap'
import { getLocations } from '../../../../../services/locationsService'
import custom from './custom.svg'

class Place extends Component {
  constructor(props) {
    super(props)
    this.state = {
      location: undefined,
      loading: false,
    }
  }

  componentDidMount() {
    const body = {
      custom_location_ids: [],
      google_maps_location_ids: [],
    }

    if (!this.props.googleId) body.custom_location_ids.push(this.props.customId)
    else body.google_maps_location_ids.push(this.props.googleId)

    this.setState({ loading: true })

    getLocations(this.props.token, body).then(res => {
      this.setState({
        loading: false,
        location:
          res.custom_location_infomation[0] ||
          res.google_maps_location_information[0],
      })
    })
  }

  getLocationName = () => {
    const loc = this.state.location

    return loc.name || loc.details.name
  }

  getLocationAddress = () => {
    const loc = this.state.location

    return loc.address || loc.details.formatted_address
  }

  getImage = () => {
    const loc = this.state.location
    if (!loc || !loc.details) return custom

    return loc.details.icon
  }

  getScore = name => {
    const r = this.state.location.rating

    return Math.round(r[name] / r[name + '_votes'])
  }

  render() {
    const loc = this.state.location

    if (!loc) return <div />

    return (
      <div className="row">
        <div className="col-1 d-flex justify-content-center">
          <div
            style={{
              display: '-webkit-flex',
              alignItems: 'center',
            }}
          >
            <img
              src={this.getImage()}
              width="25px"
              className="rounded-circle"
              alt=""
            />
          </div>
        </div>
        <div className="flex-column mx-0 col-8">
          <div className="w-100">{this.getLocationName()}</div>
          <div className="small-string">{this.getLocationAddress()}</div>
        </div>
        {this.props.showRating && (
          <div className="col-3 mx-0 d-flex justify-content-end">
            <OverlayTrigger
              key={this.getLocationAddress()}
              overlay={
                <Tooltip id={`tooltip-${this.getLocationAddress()}`}>
                  <span>{'Price ' + this.getScore('price') + '/100'}</span>
                  <br />
                  <span>{'Quality ' + this.getScore('taste') + '/100'}</span>
                  <br />
                  <span>
                    {'Quantity ' + this.getScore('amount_of_food') + '/100'}
                  </span>
                  <br />
                  <span>
                    {'Wating time ' + this.getScore('waiting_time') + '/100'}
                  </span>
                </Tooltip>
              }
            >
              <div
                style={{
                  display: '-webkit-flex',
                  alignItems: 'center',
                }}
              >
                {Math.round(loc.rating.overall_score) + '/100'}
              </div>
            </OverlayTrigger>
          </div>
        )}
      </div>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default connect(mapStateToProps, null)(Place)

import React, { Component } from 'react'
import { Spinner } from 'react-bootstrap'
import './Loading.css'

export default class Loading extends Component {
  render() {
    return this.props.show ? (
      <div className="overlay">
        <Spinner animation="border" variant="dark" />
      </div>
    ): <div/>
  }
}

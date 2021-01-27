import React, { Component } from 'react'
import './Mask.css'

export default class Mask extends Component {
  render() {
    return this.props.show ? <div className="overlay"></div> : <div />
  }
}

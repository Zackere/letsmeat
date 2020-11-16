import React, { Component } from 'react'

export default class Group extends Component {
  constructor(param) {
    super(param)
    this.state = {
      id: param.match.params.id.substring(1),
    }
  }

  render() {
    return <div>{'group '+this.state.id}</div>
  }
}

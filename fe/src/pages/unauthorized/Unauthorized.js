import React, { Component } from 'react'
import { Link } from 'react-router-dom'
import { connect } from 'react-redux'

class Unauthorized extends Component {
  render() {
    return (
      <div>
        <h1>403 - You Shall Not Pass</h1>
        <p>
          <Link to="/">Back to Home</Link>
        </p>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  ...state,
})

export default connect(mapStateToProps, null)(Unauthorized)

import React, { Component } from 'react'
import { Link } from 'react-router-dom'
import { connect } from 'react-redux'

class Unauthorized extends Component {
  render() {
    return (
      <div className="m-5">
        <h1 style={{ color: 'white' }}>403 - You Shall Not Pass</h1>
        <p>
          <Link to="/" style={{ fontSize: '1.5rem' }}>
            Back to Home
          </Link>
        </p>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  ...state,
})

export default connect(mapStateToProps, null)(Unauthorized)

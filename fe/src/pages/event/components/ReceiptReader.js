import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withToastManager } from 'react-toast-notifications'
import ImageUploader from 'react-images-upload'

import { Button } from 'react-bootstrap'

import Loading from '../../../common/loading/Loading'

import './ReceiptReader.css'

class ReceiptReader extends Component {
  constructor(props) {
    super(props)
    this.state = {
      loading: false,
      picture: [],
    }
  }

  selectPicture = picture => {
        this.setState({ picture })
  }

  render() {
    return (
      <div className="d-flex flex-column h-25">
        <ImageUploader
          withIcon={false}
          withPreview
          singleImage
          label=""
          buttonText="Choose receipt photo"
          onChange={this.selectPicture}
          imgExtension={['.jpg', '.gif', '.png', '.gif']}
          maxFileSize={5242880}
        />

        <Button
          variant="link"
          className="rounded-0"
          disabled={!this.state.picture.length}
        >
          Process receipt
        </Button>
      </div>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(ReceiptReader))

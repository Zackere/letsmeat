import React, { Component } from 'react'
import { connect } from 'react-redux'
import { Form, ListGroup, Button, InputGroup, Modal } from 'react-bootstrap'
import { BiSearch } from 'react-icons/bi'
import { withToastManager } from 'react-toast-notifications'
import { searchLocation } from '../../../../../services/locationsService'
import CustomPlaceModal from './CustomPlaceModal'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'react-datepicker/dist/react-datepicker.css'

class PlaceModal extends Component {
  constructor(props) {
    super(props)

    this.state = {
      places: [],
      customOpened: false,
    }
  }

  closeModal = () => {
    this.setState({ places: [] })
    this.props.closeModal()
  }

  addPlace = p => {
    this.closeModal()
    this.props.addGooglePlace(p)
  }

  openCustomModal = () => this.setState({ customOpened: true, places:[] })

  closeCustomModal = () => {
    this.closeModal()
    this.setState({ customOpened: false })
  }

  getPlaces = e => {
    if (e.target.value.length < 3) return
    searchLocation(
      this.props.token,
      this.props.groupId,
      e.target.value,
      this.props.sessiontoken
    ).then(res =>
      this.setState({ places: res.google_maps_locations_predictions || [] })
    )
  }

  render() {
    const places = this.state.places
    return (
      <>
        <CustomPlaceModal
          show={this.state.customOpened}
          addCustomPlace={this.props.addCustomPlace}
          closeModal={this.closeCustomModal}
        />
        <Modal
          show={this.props.show}
          aria-labelledby="contained-modal-title-vcenter"
          onHide={this.closeModal}
          centered
        >
          <Modal.Header closeButton>
            <Modal.Title id="contained-modal-title-vcenter">
              Add new meeting place
            </Modal.Title>
          </Modal.Header>
          <Modal.Body>
            <label className="mt-2">Meeting place</label>
            <div className="col-12">
              <Form>
                <Form.Group>
                  <InputGroup>
                    <InputGroup.Prepend>
                      <InputGroup.Text>
                        <BiSearch />
                      </InputGroup.Text>
                    </InputGroup.Prepend>
                    <Form.Control
                      type="text"
                      placeholder="Add new place"
                      onChange={this.getPlaces}
                    />
                  </InputGroup>
                </Form.Group>
              </Form>
            </div>
            <ListGroup className="list-scroll" style={{height: 'auto', maxHeight:'25vh'}}>
              {places.map(p => (
                <ListGroup.Item
                  key={p.place_id}
                  action
                  onClick={() => this.addPlace(p)}
                >
                  <span>{p.description}</span>
                </ListGroup.Item>
              ))}
            </ListGroup>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="outline-secondary" onClick={this.closeModal}>
              Close
            </Button>
            <Button variant="outline-primary" onClick={this.openCustomModal}>
              Add custom
            </Button>
          </Modal.Footer>
        </Modal>
      </>
    )
  }
}

const mapStateToProps = state => ({
  token: state.token,
})

export default withToastManager(connect(mapStateToProps, null)(PlaceModal))

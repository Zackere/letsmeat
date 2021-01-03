import React, { Component } from 'react'
import { IconContext } from 'react-icons'
import { BsFillPlusCircleFill } from 'react-icons/bs'
import { FaVoteYea } from 'react-icons/fa'
import { IoIosArrowUp, IoIosArrowDown } from 'react-icons/io'
import { Button, ListGroup } from 'react-bootstrap'
import PlaceModal from '../modals/PlaceModal'
import Place from './Place'


class Places extends Component {
  constructor(props) {
    super(props)
    this.state = {
      modalOpened: false,
    }
  }

  openModal = () => this.setState({ modalOpened: true })
  closeModal = () => this.setState({ modalOpened: false })

  render() {
    const candidates = this.props.locations
    return (
      <>
        <PlaceModal
          show={this.state.modalOpened}
          closeModal={this.closeModal}
          addGooglePlace={this.props.addGooglePlace}
          addCustomPlace={this.props.addCustomPlace}
          sessiontoken={this.props.sessiontoken}
          groupId={this.props.groupId}
        />
        <div className="d-flex flex-column">
          <div className="mr-2">
            <Button
              variant="link"
              className="no-focus float-right"
              onClick={this.openModal}
            >
              <IconContext.Provider value={{ size: '23px', color: 'white' }}>
                <BsFillPlusCircleFill />
              </IconContext.Provider>
            </Button>
            <Button
              variant="link"
              className="no-focus float-right"
              onClick={this.props.vote}
            >
              <IconContext.Provider value={{ size: '23px', color: 'white' }}>
                <FaVoteYea />
              </IconContext.Provider>
            </Button>
          </div>

          <ListGroup className="list-scroll" style={{ height: '70vh' }}>
            {candidates &&
              candidates.map((c, i) => (
                <ListGroup.Item
                  key={c.google_maps_location_id || c.custom_location_id}
                  action
                >
                  <div className="row justify-content-between pl-2">
                    <div className="col-9">
                      <Place
                        googleId={c.google_maps_location_id}
                        customId={c.custom_location_id}
                        showRating={true}
                      />
                      </div>
                    <div className="d-flex col-3">
                      <Button
                        variant="link"
                        className="no-focus ml-auto"
                        onClick={() => this.props.up(i)}
                      >
                        <IconContext.Provider
                          value={{ size: '23px', color: '#343a40' }}
                        >
                          <IoIosArrowUp />
                        </IconContext.Provider>
                      </Button>
                      <Button
                        variant="link"
                        className="no-focus ml-0"
                        onClick={() => this.props.down(i)}
                      >
                        <IconContext.Provider
                          value={{ size: '23px', color: '#343a40' }}
                        >
                          <IoIosArrowDown />
                        </IconContext.Provider>
                      </Button>
                    </div>
                  </div>
                </ListGroup.Item>
              ))}
          </ListGroup>
        </div>
      </>
    )
  }
}

export default Places

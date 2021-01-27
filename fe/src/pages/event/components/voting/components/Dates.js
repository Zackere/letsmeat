import React, { Component } from 'react'
import { IconContext } from 'react-icons'
import { BsFillPlusCircleFill } from 'react-icons/bs'
import { FaVoteYea } from 'react-icons/fa'
import { IoIosArrowUp, IoIosArrowDown } from 'react-icons/io'
import { Button, ListGroup } from 'react-bootstrap'
import DateModal from '../modals/DateModal'

import { basicFormat } from '../../../../../common/date formater/dateFormater'

class Dates extends Component {
  constructor(props) {
    super(props)

    this.state = {
      modalOpened: false,
    }
  }

  openModal = () => this.setState({ modalOpened: true })
  closeModal = () => this.setState({ modalOpened: false })

  render() {
    const candidates = this.props.dates

    return (
      <>
        <DateModal
          show={this.state.modalOpened}
          closeModal={this.closeModal}
          addDate={this.props.addDate}
          deadline = {this.props.deadline}
        />
        <div className="d-flex flex-column">
          <div className="mr-2">
            <Button
              variant="link"
              className="no-focus float-right"
              onClick={this.openModal}
            >
              <IconContext.Provider value={{ size: '23px', color: '#343a40' }}>
                <BsFillPlusCircleFill />
              </IconContext.Provider>
            </Button>
            <Button
              variant="link"
              className="no-focus float-right"
              onClick={this.props.vote}
            >
              <IconContext.Provider value={{ size: '23px', color: '#343a40' }}>
                <FaVoteYea />
              </IconContext.Provider>
            </Button>
          </div>

          <ListGroup className="list-scroll" style={{ height: '70vh' }}>
            {candidates &&
              candidates.map((c, i) => (
                <ListGroup.Item key={c} action>
                  <div className="row justify-content-between pl-2">
                    <div className="d-flex">
                      <span className="align-self-center">
                        {basicFormat(c)}
                      </span>
                    </div>
                    <div className="d-flex">
                      <Button
                        variant="link"
                        className="no-focus"
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
                        className="no-focus"
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

export default Dates

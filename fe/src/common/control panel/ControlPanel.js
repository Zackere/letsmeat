import React, { Component } from 'react'
import { IconContext } from 'react-icons'
import { BiSearch } from 'react-icons/bi'
import { BsFillPlusCircleFill } from 'react-icons/bs'
import { Row, Col, Button, Form, InputGroup } from 'react-bootstrap'

export default class ControlPanel extends Component {
  render() {
    const { buttonAction, searchName, searchAction } = this.props
    return (
      <Row className="mt-3">
        <Col xs="9">
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
                  placeholder={searchName}
                  onChange={searchAction}
                />
              </InputGroup>
            </Form.Group>
          </Form>
        </Col>
        <Col xs="1"></Col>
        <Col xs="2">
          <Button
            variant="link"
            className="float-right no-focus"
            style={{ marginRight: '25px' }}
            onClick={buttonAction}
          >
            <IconContext.Provider value={{ size: '27px', color: this.props.buttonColor || '#343a40' }}>
              <BsFillPlusCircleFill />
            </IconContext.Provider>
          </Button>
        </Col>
      </Row>
    )
  }
}

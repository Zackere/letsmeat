import React, { Component } from 'react'
import { Row, Col, Button, Form } from 'react-bootstrap'

export default class ControlPanel extends Component {
  render() {
    const { buttonName, buttonAction, searchName, searchAction } = this.props
    return (
      <Row className="mt-3">
        <Col xs="6">
          <Form>
            <Form.Group>
              <Form.Control
                className="rounded-0"
                type="text"
                placeholder={searchName}
                onChange={searchAction}
              />
            </Form.Group>
          </Form>
        </Col>
        <Col xs="4" />
        <Col xs="2">
          <Button
            variant="outline-dark"
            className="float-right rounded-0"
            onClick={buttonAction}
          >
            {buttonName}
          </Button>
        </Col>
      </Row>
    )
  }
}

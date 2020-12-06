import React, { Component } from 'react'
import { connect } from 'react-redux'
import { withRouter } from 'react-router-dom'
import { ListGroup, Container } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

import Bar from '../../common/bar/Bar'
import ControlPanel from '../../common/control panel/ControlPanel'
import { getGroups } from '../../services/groupsServices'

class Groups extends Component {
  state = {
    groups: [],
    all: [],
  }

  componentDidMount() {
    getGroups(this.props.user.googleId).then(groups =>
      this.setState({ groups: groups, all: groups })
    )
  }

  goToGroupPage(id) {
    this.props.history.push('/group:' + id)
  }

  buttonAction = () => console.log('click')

  searchAction = event => {
    const newList = this.state.all.filter(g =>
      g.name.startsWith(event.target.value)
    )

    this.setState({ groups: newList })
  }

  render() {
    return (
      <Container>
        <Bar />
        <ControlPanel
          buttonName="Add"
          buttonAction={this.buttonAction}
          searchName="Find group"
          searchAction={this.searchAction}
        />
        <ListGroup variant="flush">
          {this.state.groups.map(group => (
            <ListGroup.Item
              key={group.id}
              action
              onClick={() => this.goToGroupPage(group.id)}
            >
              <div className="row">
                <div className="col-sm">{group.name}</div>
                <div className="col-sm text-right">{group.date}</div>
              </div>
            </ListGroup.Item>
          ))}
        </ListGroup>
      </Container>
    )
  }
}

const mapStateToProps = state => ({
  user: state.user,
})

export default withRouter(connect(mapStateToProps, null)(Groups))

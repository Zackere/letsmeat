import React, { useContext, useEffect, useState } from 'react';
import {
  ActivityIndicator,
  Button, Caption, Card,
  Paragraph, Subheading
} from 'react-native-paper';
import {
  acceptInvitation, getGroupInfo, getUsersInfo, rejectInvitation
} from '../Requests';
import { store } from '../Store';

export const Invitation = ({ invitation, full = false }) => {
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);
  const [group, setGroup] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, invitation.from_id).then((users) => setUser(users[0]));
    getGroupInfo({ state }, invitation.group_id).then(setGroup);
  }, []);

  return (
    <Card style={{ margin: 5 }}>
      <Card.Content>
        { (user && group)
          ? (
            <Paragraph>
              <Subheading>{user.name}</Subheading>
              {'\t'}
              <Caption>invites you to join</Caption>
              {'\n'}
              <Subheading>{group.name}</Subheading>
            </Paragraph>
          )
          : <ActivityIndicator />}
      </Card.Content>
      <Card.Actions>
        <Button
          color="red"
          onPress={() => {
            rejectInvitation({ state }, group.id).then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }));
          }}
        >
          Refuse
        </Button>
        <Button onPress={() => {
          acceptInvitation({ state }, group.id).then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }));
        }}
        >
          Accept
        </Button>
      </Card.Actions>
    </Card>
  );
};

export const Debt = ({ debt, full = false }) => {
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, debt.from_id).then((users) => setUser(users[0]));
  }, []);

  return (
    <Card style={{ margin: 5 }}>
      <Card.Content>
        { (user)
          ? (
            <Paragraph>
              <Subheading>{user.name}</Subheading>
              {'\t'}
              <Caption>wants you to pay</Caption>
              {'\n'}
              <Subheading>{(debt.amount / 100).toFixed(2)}</Subheading>
            </Paragraph>
          )
          : <ActivityIndicator />}
      </Card.Content>
      <Card.Actions>
        <Button
          color="red"
          onPress={() => {
            rejectInvitation({ state }, group.id).then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }));
          }}
        >
          Refuse
        </Button>
        <Button onPress={() => {
          acceptInvitation({ state }, group.id).then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }));
        }}
        >
          Accept
        </Button>
      </Card.Actions>
    </Card>
  );
};

export const Notification = ({ item, full = false }) => (item.kind === 'invitation' ? <Invitation invitation={item} full={full} /> : <Debt debt={item} full={full} />);

export default Notification;

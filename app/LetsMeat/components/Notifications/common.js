import React, { useContext, useEffect, useState } from 'react';
import {
  ActivityIndicator,
  Button, Caption, Card,
  Paragraph, Subheading
} from 'react-native-paper';
import {
  acceptDebt, acceptInvitation, getGroupInfo, getUsersInfo, rejectDebt, rejectInvitation
} from '../Requests';
import { store } from '../Store';

const NotificationAction = ({ acceptAction, rejectAction }) => {
  const [active, setActive] = useState(false);

  return (
    <Card.Actions>
      <Button
        disabled={active}
        color="red"
        onPress={() => {
          setActive(true);
          rejectAction().finally(() => setActive(false));
        }}
      >
        Refuse
      </Button>
      <Button
        disabled={active}
        onPress={() => {
          setActive(true);
          acceptAction().finally(() => setActive(false));
        }}
      >
        Accept
      </Button>
    </Card.Actions>
  );
};

const NotificationContent = ({ content, loading }) => (
  <Card.Content>
    { loading
      ? <ActivityIndicator />
      : { content }}
  </Card.Content>
);

export const Invitation = ({ invitation, full = false }) => {
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);
  const [group, setGroup] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, invitation.from_id).then((users) => setUser(users[0]));
    getGroupInfo({ state }, invitation.group_id).then(setGroup);
  }, [state, invitation.from_id, invitation.group_id]);

  return (
    <Card style={{ margin: 5 }}>
      <NotificationContent
        loading={!user || !group}
        content={(!user || !group) ? (
          <Paragraph>
            <Subheading>{user.name}</Subheading>
            {'\t'}
            <Caption>invites you to join</Caption>
            {'\n'}
            <Subheading>{group.name}</Subheading>
          </Paragraph>
        ) : null}
      />
      <NotificationAction
        rejectAction={() => rejectInvitation({ state }, group.id)
          .then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }))}
        acceptAction={() => acceptInvitation({ state }, group.id)
          .then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }))}
      />
    </Card>
  );
};

export const Debt = ({ debt, full = false }) => {
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, debt.from_id).then((users) => setUser(users[0]));
  }, [debt.from_id, state]);

  return (
    <Card style={{ margin: 5 }}>
      <NotificationContent
        loading={!user}
        content={user ? (
          <Paragraph>
            <Subheading>{user.name}</Subheading>
            {'\t'}
            <Caption>wants you to pay</Caption>
            {'\n'}
            <Subheading>{(debt.amount / 100).toFixed(2)}</Subheading>
          </Paragraph>
        ) : null}
      />
      <NotificationAction
        rejectAction={() => rejectDebt({ state }, debt.id)
          .then(() => dispatch({ type: 'REMOVE_DEBT', debtId: debt.id }))}
        acceptAction={() => acceptDebt({ state }, debt.id)
          .then(() => dispatch({ type: 'REMOVE_DEBT', debtId: debt.id }))}
      />
    </Card>
  );
};

export const Notification = (
  {
    item,
    full = false
  }
) => (item.kind === 'invitation'
  ? <Invitation invitation={item} full={full} />
  : <Debt debt={item} full={full} />);

export default Notification;

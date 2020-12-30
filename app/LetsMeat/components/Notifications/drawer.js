import React, { useContext, useEffect, useState } from 'react';
import { View } from 'react-native';
import {
  ActivityIndicator,
  Button, Caption, Card,
  IconButton, Paragraph, Subheading
} from 'react-native-paper';
import {
  acceptInvitation, getGroupInfo, getInvitations, getPendingDebts, getUsersInfo, rejectInvitation
} from '../Requests';
import { store } from '../Store';

export const InvitationButton = ({ invitation }) => {
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

export const DebtButton = ({ debt }) => {
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);
  const [group, setGroup] = useState(null);

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

export const Notification = ({ item }) => (item.kind === 'invitation' ? <InvitationButton invitation={item} /> : <DebtButton debt={item} />);

const getNotificationTimestamp = (notification) => (notification.kind === 'invitation'
  ? new Date(notification.sent).getTime()
  : new Date(notification.timestamp).getTime());

const Notifications = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  const notifications = [...state.invitations, ...state.debts]
    .sort((a, b) => getNotificationTimestamp(b) - getNotificationTimestamp(a));

  console.log(notifications);

  const refreshNotifications = () => {
    const APIactions = [
      getInvitations({ state }).then((invitations) => {
        dispatch({ type: 'SET_INVITATIONS', payload: invitations.map((i) => ({ ...i, kind: 'invitation' })) });
      }),
      getPendingDebts({ state }).then((debts) => {
        dispatch({ type: 'SET_DEBTS', payload: debts.map((i) => ({ ...i, kind: 'debt' })) });
      })];
    Promise.all(APIactions).then(() => console.log(state.debts));
  };

  return (
    <View>
      <View style={{ flexDirection: 'row', margin: 5 }}>
        <Subheading style={{ margin: 10 }}>Notifications</Subheading>
        <IconButton
          icon="refresh"
          size={20}
          style={{ position: 'absolute', top: -9, right: 0 }}
          onPress={refreshNotifications}
        />
      </View>
      {notifications.length > 0
        ? (
          <>
            {notifications.slice(0, 2)
              .map((item) => (
                <Notification
                  item={item}
                  key={`${item.group_id}${item.id}`}
                />
              ))}
            {notifications.length > 2 && <Button onPress={() => navigation.navigate('Notifications')}>Show all</Button>}
          </>
        )
        : (
          <View style={{ alignItems: 'center', marginBottom: 10 }}>
            <Caption>You have no pending notifications</Caption>
          </View>
        )}
    </View>
  );
};

export default Notifications;

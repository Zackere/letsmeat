import React, {
  useContext, useEffect, useState, useRef
} from 'react';
import { StyleSheet } from 'react-native';
import {
  ActivityIndicator,
  Button, Caption, Card,
  Paragraph, Subheading
} from 'react-native-paper';
import { formatAmount } from '../../helpers/money';
import {
  acceptDebt, acceptInvitation, getGroupInfo, getImagesInfo, getUsersInfo, rejectDebt, rejectInvitation
} from '../Requests';
import { store } from '../Store';

const NotificationAction = ({ acceptAction, rejectAction, full }) => {
  const mounted = useRef(false);
  const [active, setActive] = useState(false);
  useEffect(() => {
    mounted.current = true;
    return () => { mounted.current = false; };
  });

  const finishAction = () => {
    if (!mounted.current) return;
    setActive(false);
  };

  return (
    <Card.Actions style={{ justifyContent: full ? 'space-around' : undefined }}>
      <Button
        disabled={active}
        color="red"
        onPress={() => {
          setActive(true);
          rejectAction().finally(finishAction);
        }}
      >
        Refuse
      </Button>
      <Button
        disabled={active}
        onPress={() => {
          setActive(true);
          acceptAction().finally(finishAction);
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
      : content }
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

  const loaded = user && group;

  return (
    <Card style={{ margin: 5 }}>
      {(full && loaded) ? <Card.Title>{`${user.name} invites you to join ${group.name}`}</Card.Title>
        : (
          <NotificationContent
            loading={!loaded}
            content={loaded ? (
              <Paragraph>
                <Subheading>{user.name}</Subheading>
                {'\t'}
                <Caption>invites you to join</Caption>
                {'\n'}
                <Subheading>{group.name}</Subheading>
              </Paragraph>
            ) : null}
          />
        )}
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
  const mounted = useRef(false);
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);
  const [image, setImage] = useState(null);

  useEffect(() => {
    mounted.current = true;
    getUsersInfo({ state }, debt.to_id).then((users) => {
      if (!mounted.current) return;
      setUser(users[0]);
    });
    if (full && debt.image_id) {
      getImagesInfo({ state }, debt.image_id).then(([response]) => {
        if (!mounted.current) return;
        setImage(response);
      });
    }
    return () => { mounted.current = false; };
  }, [debt.to_id, state, full, debt.image_id]);

  const request = (debt.debt_type === 0) ? 'wants you to pay' : 'wants you to confirm they transferred';

  return (
    <Card style={full ? styles.full : styles.small}>
      {full && user && <Card.Title multiline titleNumberOfLines={3} title={`${user.name} ${request} ${formatAmount(debt.amount)}`} />}
      <NotificationContent
        loading={!user}
        content={user ? (
          <>
            {full ? (
              <Paragraph margin={5}>
                {debt.description}
              </Paragraph>
            )
              : (
                <Paragraph>
                  <Subheading>{user.name}</Subheading>
                  {'\t'}
                  <Caption>{request}</Caption>
                  {'\n'}
                  <Subheading>{formatAmount(debt.amount)}</Subheading>
                </Paragraph>
              )}
          </>
        ) : null}
      />
      <NotificationAction
        full={full}
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

const styles = StyleSheet.create({
  full: {
    margin: 10,
    backgroundColor: 'rgba(230, 230, 230, 0.9)'
  },
  small: {
    margin: 5
  }
});

export default Notification;

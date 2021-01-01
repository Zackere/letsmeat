import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet } from 'react-native';
import {
  Button, Card
} from 'react-native-paper';
import { getUsersInfo } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

const DebtCard = ({ value = 1500 }) => {
  if (!value) return null;
  return (
    <Card style={{ marginHorizontal: 30, marginTop: -10 }}>
      <Button>
        {value > 0 ? `Owes you ${value / 100}` : `You owe ${-value / 100}`}
      </Button>
    </Card>
  );
};

const getDebtValue = (state, id) => {
  const myId = state.user.id;
  if (myId === id) return null;
  if (state.group.debts[id] && state.group.debts[id][myId]) return state.group.debts[id][myId];
  if (state.group.debts[myId] && state.group.debts[myId][id]) return state.group.debts[myId][id];
  return 0;
};

const GroupMembers = ({ members, navigation, debts = [] }) => {
  const { state } = useContext(store);
  const [membersInfo, setMembersInfo] = useState(members);

  useEffect(() => {
    getUsersInfo({ state }, members.map((m) => m.id)).then(setMembersInfo);
  }, [members, state]);

  const MEMBERS_TO_SHOW = 3;

  return (
    <Card
      elevation={0}
      style={styles.emptyCard}
    >
      <Card.Title title="Members" />
      <Card.Content>
        {membersInfo.slice(0, MEMBERS_TO_SHOW).map((m) => (
          <React.Fragment key={m.id}>
            <UserCard
              user={m}
            />
            <DebtCard
              value={getDebtValue(state, m.id)}
            />
          </React.Fragment>
        ))}
      </Card.Content>
      <Card.Actions style={{ justifyContent: 'space-evenly' }}>
        <Button onPress={() => navigation.navigate('Invite')}>
          <Icon name="plus" size={25} />
        </Button>
        {
          (membersInfo.length > MEMBERS_TO_SHOW)
            ? (
              <Button onPress={() => navigation.navigate('Invite')}>
                <Icon name="dots-horizontal" size={25} />
              </Button>
            )
            : null
        }
      </Card.Actions>
    </Card>
  );
};
const styles = StyleSheet.create({
  progressBar: {
    width: '100%',
    height: '100%',
    justifyContent: 'center',
    alignItems: 'center'
  },
  groupsContainer: {
    width: '100%',
    height: '100%'
  },
  fab: {
    position: 'absolute',
    margin: 30,
    right: 0,
    bottom: 0,
  },
  emptyCard: {
    margin: 10
  },
  user: {
    margin: 5
  },
  cardButton: {
    margin: 25,
    height: 50,
  },
  delete: {
    backgroundColor: '#fc3503'
  },
  leave: {
    backgroundColor: '#fc3503'
  }
});

export default GroupMembers;

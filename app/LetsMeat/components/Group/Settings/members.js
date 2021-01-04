import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet } from 'react-native';
import {
  Button, Card, Surface
} from 'react-native-paper';
import { getUsersInfo } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';
import DebtCard from '../../Debts';
import BackgroundContainer from '../../Background';

const getDebtValue = (state, id) => {
  const myId = state.user.id;
  if (myId === id) return null;
  if (state.group.debts[id] && state.group.debts[id][myId]) return state.group.debts[id][myId];
  if (state.group.debts[myId] && state.group.debts[myId][id]) return state.group.debts[myId][id];
  return 0;
};

export const GroupMembers = ({
  members,
  navigation,
  displayContainer = true,
  membersToDisplay = 3,
  showAll = false
}) => {
  const { state } = useContext(store);
  const [membersInfo, setMembersInfo] = useState(members);

  useEffect(() => {
    getUsersInfo({ state }, members.map((m) => m.id)).then(setMembersInfo);
  }, [members, state]);

  const membersSlice = showAll ? membersInfo : membersInfo.slice(0, membersToDisplay);

  const list = membersSlice.map((m) => (
    <React.Fragment key={m.id}>
      <UserCard
        user={m}
        actions={m.id !== state.user.id ? (
          <Button onPress={
        () => navigation.navigate('SendTransfer', {
          user: m,
          amount: getDebtValue(state, m.id)
        })
      }
          >
            Send Transfer
          </Button>
        ) : undefined}
      />
      <DebtCard
        value={getDebtValue(state, m.id)}
      />
    </React.Fragment>
  ));

  return (
    <>
      {
      displayContainer
        ? (
          <Card
            elevation={1}
            style={styles.emptyCard}
          >
            <Card.Title title="Members" />
            <Card.Content>
              {list}
            </Card.Content>
            <Card.Actions style={{ justifyContent: 'space-evenly' }}>
              <Button onPress={() => navigation.navigate('Invite')}>
                <Icon name="plus" size={25} />
              </Button>
              {
            (membersInfo.length > membersToDisplay)
              ? (
                <Button onPress={() => navigation.navigate('Members')}>
                  <Icon name="dots-horizontal" size={25} />
                </Button>
              )
              : null
        }
            </Card.Actions>
          </Card>
        ) : list
  }
    </>
  );
};

export const MembersScreen = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  return (
    <BackgroundContainer>
      <GroupMembers
        members={state.group.users}
        debts={state.group.debts}
        navigation={navigation}
        displayContainer={false}
        showAll
      />
    </BackgroundContainer>
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
    margin: 10,
    backgroundColor: 'rgba(200, 200, 200, 0.9)'
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
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet } from 'react-native';
import {
  Button, Card
} from 'react-native-paper';
import { getUsersInfo } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

const Locations = ({ locations, navigation }) => {
  const { state } = useContext(store);
  // const [membersInfo, setMembersInfo] = useState(members);

  // useEffect(() => {
  //   getUsersInfo({ state }, members.map((m) => m.id)).then(setMembersInfo);
  // }, [members, state]);

  // const MEMBERS_TO_SHOW = 3;

  return (
    <Card
      elevation={0}
      style={styles.emptyCard}
    >
      <Card.Title title="Locations" />
      <Card.Content>
        {membersInfo.slice(0, MEMBERS_TO_SHOW).map((m) => (
          <UserCard
            key={m.id}
            user={m}
          />
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

export default Locations;

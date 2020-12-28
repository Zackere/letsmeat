import React, {
  useContext,
  useEffect, useState
} from 'react';
import {
  StyleSheet
} from 'react-native';
import {
  ActivityIndicator,
  Card
} from 'react-native-paper';
import { store } from '../../Store';
import { getUsersInfo } from '../../Requests';
import UserCard from '../../User';

const Creator = ({ userId }) => {
  const { state } = useContext(store);
  const [userInfo, setUserInfo] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, userId).then((users) => {
      setUserInfo(users[0]);
    });
  }, [state, userId]);

  return (
    <Card elevation={0} style={styles.section}>
      <Card.Title title="Creator" />
      {userInfo
        ? <UserCard user={userInfo} />
        : <ActivityIndicator />}
    </Card>
  );
};

const styles = StyleSheet.create({
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20
  },
  container: {
    width: '100%',
    height: '100%'
  },
  section: {
    margin: 10
  },
  card: {
    margin: 25
  },
  addButton: {
    marginBottom: 10
  },
  timeCard: {
    margin: 5,
    justifyContent: 'center'
  }
});

export default Creator;

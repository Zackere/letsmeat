import React, { useContext, useState, useEffect } from 'react';
import { StyleSheet, View, ScrollView } from 'react-native';
import {
  Avatar, Button, Caption, Card, Dialog, Portal, TextInput, ActivityIndicator
} from 'react-native-paper';
import { store } from '../Store';
import { getUsersInfo } from '../Requests';

const MIN_OPACITY = 0.3;

const createPrefStyle = (value, maxWidth = 100) => (
  {
    ...styles.bar,
    width: maxWidth * ((value + 10) / 110),
    opacity: MIN_OPACITY + (value * (1 - MIN_OPACITY)) / 100
  });

const Prefs = ({ prefs }) => {
  const [barWidth, setBarWidth] = useState(1);
  return (
    <View style={{
      width: '100%', flexDirection: 'row', justifyContent: 'space-between'
    }}
    >
      <View style={{ width: '50%' }}>
        <Caption>Price</Caption>
        <View style={createPrefStyle(prefs.price, barWidth - 10)} />
        <Caption>Waiting Time</Caption>
        <View style={createPrefStyle(prefs.waiting_time, barWidth - 10)} />
      </View>
      <View
        style={{ width: '50%' }}
        onLayout={(event) => {
          const { width } = event.nativeEvent.layout;
          setBarWidth(width);
        }}
      >
        <Caption>Portion Size</Caption>
        <View style={createPrefStyle(prefs.amount_of_food, barWidth)} />
        <Caption>Taste</Caption>
        <View style={createPrefStyle(prefs.taste, barWidth)} />
      </View>
    </View>
  );
};

const UserCard = ({
  user, style, actions, onPress
}) => (
  <Card key={user.id} style={{ ...styles.userCard, ...style }} onPress={onPress}>
    <Card.Title title={user.name} subtitle={user.email} />
    <Card.Content style={styles.content}>
      <View style={styles.leftContent}>
        <Avatar.Image source={{ uri: user.picture_url }} />
      </View>
      <View style={styles.rightContent}>
        {user.prefs
          ? (
            <Prefs prefs={user.prefs} />
          ) : <Caption style={{ fontStyle: 'italic', alignSelf: 'center', marginTop: 20 }}>User Preferences not available</Caption>}
      </View>
    </Card.Content>
    {actions
    && (
    <Card.Actions>
      {actions}
    </Card.Actions>
    )}
  </Card>
);

export const UserPicker = ({
  userIds, dialogVisible, setDialogVisible, onDismiss, setUser
}) => {
  const [query, setQuery] = useState('');
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const hideDialog = () => setDialogVisible(false);
  const { state } = useContext(store);

  const filteredUsers = users.filter((u) => u.name.toLowerCase().includes(query.toLowerCase()));

  useEffect(() => {
    getUsersInfo({ state }, userIds).then((users) => {
      setUsers(users);
      console.log(users);
      setLoading(false);
    });
  }, [state, userIds]);

  return (
    <Portal>
      <Dialog visible={dialogVisible} contentContainerStyle={styles.container} onDismiss={onDismiss}>
        <Dialog.Content>
          <TextInput
            mode="outlined"
            label="Search user"
            onChangeText={setQuery}
            value={query}
          />
          {loading ? <ActivityIndicator />
            : (
              <ScrollView>
                { filteredUsers.map((u) => (
                  <UserCard
                    user={u}
                    key={u.id}
                    onPress={() => {
                      setUser(u);
                      onDismiss();
                    }}
                  />
                ))}
              </ScrollView>
            )}
        </Dialog.Content>
      </Dialog>
    </Portal>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  userCard: {
    margin: 20,
    backgroundColor: 'rgba(240, 240, 240, 0.9)'
  },
  content: {
    marginTop: 10,
    marginBottom: 20,
    display: 'flex',
    flexDirection: 'row'
  },
  leftContent: {
    marginLeft: 5,
    flexGrow: 1,
    // backgroundColor: 'red'
  },
  rightContent: {
    marginTop: -15,
    marginLeft: 20,
    marginRight: 0,
    maxWidth: '70%',
    // backgroundColor: 'black',
    display: 'flex',
    flexDirection: 'column'
  },
  bar: {
    borderRadius: 5,
    backgroundColor: '#0a54c9',
    height: 20
  }

});

export default UserCard;

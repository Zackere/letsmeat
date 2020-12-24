import React from 'react';
import { StyleSheet, View, Text } from 'react-native';
import { Avatar, Card, Caption } from 'react-native-paper';

const MIN_OPACITY = 0.3;

const createPrefStyle = (value) => ({ ...styles.bar, width: value + 10, opacity: MIN_OPACITY + (value * (1 - MIN_OPACITY)) / 100 });

const Prefs = ({ prefs }) => (
  <View style={{ width: '100%', flexDirection: 'row', justifyContent: 'space-between' }}>
    <View style={{ width: '50%' }}>
      <Caption>Price</Caption>
      <View style={createPrefStyle(prefs.price)} />
      <Caption>Waiting Time</Caption>
      <View style={createPrefStyle(prefs.waiting_time)} />
    </View>
    <View style={{ width: '50%' }}>
      <Caption>Portion Size</Caption>
      <View style={createPrefStyle(prefs.amount_of_food)} />
      <Caption>Taste</Caption>
      <View style={createPrefStyle(prefs.taste)} />
    </View>
  </View>
);

const UserCard = ({ user, style, actions }) => {
  return (
    <Card key={user.id} style={{ ...styles.userCard, ...style }}>
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
};

const styles = StyleSheet.create({
  userCard: {
    margin: 20
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
    marginLeft: 30,
    marginRight: 0,
    flexGrow: 3,
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

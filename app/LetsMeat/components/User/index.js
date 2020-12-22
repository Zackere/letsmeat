import React from 'react';
import { StyleSheet, View } from 'react-native';
import { Avatar, Card } from 'react-native-paper';

const UserCard = ({ user, style, actions }) => (
  <Card key={user.id} style={{ ...styles.userCard, ...style }}>
    <Card.Title title={user.name} subtitle={user.email} />
    <Card.Content style={styles.content}>
      <View style={styles.leftContent}>
        <Avatar.Image source={{ uri: user.picture_url }} />
      </View>
      <View style={styles.rightContent} />
    </Card.Content>
    {actions
    && (
    <Card.Actions>
      {actions}
    </Card.Actions>
    )}
  </Card>
);

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
    backgroundColor: 'red'
  },
  rightContent: {
    marginLeft: 5,
    flexGrow: 3,
    backgroundColor: 'black',
    display: 'flex',
    flexDirection: 'column'
  }

});

export default UserCard;

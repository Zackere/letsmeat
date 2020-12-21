import React from 'react';
import { StyleSheet } from 'react-native';
import { Avatar, Card } from 'react-native-paper';

const UserCard = ({ user, style, actions }) => (
  <Card key={user.id} style={{ ...styles.userCard, ...style }}>
    <Card.Title title={user.name} />
    <Card.Content>
      <Avatar.Image source={{ uri: user.picture_url }} />
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
  }
});

export default UserCard;

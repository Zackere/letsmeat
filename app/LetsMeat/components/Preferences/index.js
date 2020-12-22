import React, { useContext, useEffect } from 'react';
import { View, StyleSheet, Text } from 'react-native';
import { Surface } from 'react-native-paper';
import { store } from '../Store';
import UserCard from '../User';

const Preferences = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  useEffect(() => { console.log(<UserCard user={state.user} />); }, []);

  return (
    <Surface style={styles.container}>
      <UserCard user={{ ...state.user, picture_url: state.user.photo }} />
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  title: {
    fontSize: 50,
    margin: 50
  }
});

export default Preferences;

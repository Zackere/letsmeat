import React, { useContext, useState, useCallback, useEffect } from 'react';
import {
  Text, View, StyleSheet, ScrollView, RefreshControl
} from 'react-native';
import { Card, Surface, Title } from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { TouchableHighlight } from 'react-native-gesture-handler';
import { Header } from '../Header';
import { store } from '../../Store';
import { getEventInfo, getGroupInfo } from '../../Requests';

const Creator = ({ user }) =>
  // placeholder
  (
    <Text>
      {user}
    </Text>
  );

const Deadline = ({ time }) =>
  // placeholder
  (
    <Text>
      {time}
    </Text>
  );

const EventView = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [eventDetails, setEventDetails] = useState({ ...state.event });

  useEffect(() => {
    console.log('using callback xD');
    getEventInfo({ state }, state.event.id).then((event) => {
      console.log(event);
      setEventDetails({ ...eventDetails, ...event });
    });
  }, []);

  return (
    <Surface style={styles.container}>
      <Title>{state.event.name}</Title>
      <Creator user={eventDetails.creator_id} />
      <Deadline time={eventDetails.deadline} />
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  card: {
    margin: 25
  }
});

export default EventView;

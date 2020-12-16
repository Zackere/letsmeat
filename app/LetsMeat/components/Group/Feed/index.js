import React, { useContext } from 'react';
import {
  Text, View, StyleSheet, ScrollView, RefreshControl
} from 'react-native';
import { Card, Surface } from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { TouchableHighlight } from 'react-native-gesture-handler';
import { Header } from '../Header';
import { store } from '../../Store';
import { getGroupInfo } from '../../Requests';
import FeedContent from './feedContent';
import EventView from './event';

const Stack = createStackNavigator();

const Feed = () => {
  const { state, dispatch } = useContext(store);

  return (
    <Stack.Navigator
      initialRouteName="Feed"
      headerMode="screen"
      screenOptions={{
        header: ({ scene, previous, navigation }) => (
          <Header scene={scene} previous={previous} navigation={navigation} />
        ),
      }}
    >
      <Stack.Screen
        name="Feed"
        component={FeedContent}
        options={{ headerTitle: state.group.name }}
      />
      <Stack.Screen
        name="Event"
        component={EventView}
        options={{ headerTitle: state.group.name }}
      />
    </Stack.Navigator>
  );
};



export default Feed;

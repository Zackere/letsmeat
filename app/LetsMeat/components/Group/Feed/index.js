import { createStackNavigator } from '@react-navigation/stack';
import React, { useContext } from 'react';
import { store } from '../../Store';
import { Header } from '../../Header';
import EventView from './event';
import FeedContent from './feedContent';

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

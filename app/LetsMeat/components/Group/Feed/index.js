import { createStackNavigator } from '@react-navigation/stack';
import React, { useContext, useLayoutEffect } from 'react';
import { getFocusedRouteNameFromRoute } from '@react-navigation/native';
import { store } from '../../Store';
import { Header } from '../../Header';
import EventView from './event';
import FeedContent from './feedContent';
import VoteTime from './voteTime';
import AddLocation from './addLocation';
import VoteLocation from './voteLocation';

const Stack = createStackNavigator();

const SCREENS_WITHOUT_TABS = new Set(['Event', 'VoteTime', 'VoteLocation', 'AddLocation']);

const Feed = ({ navigation, route }) => {
  const { state } = useContext(store);

  useLayoutEffect(() => {
    const screenName = getFocusedRouteNameFromRoute(route);
    if (SCREENS_WITHOUT_TABS.has(screenName)) {
      navigation.setOptions({ tabBarVisible: false });
    } else {
      navigation.setOptions({ tabBarVisible: true });
    }
  }, [navigation, route]);

  return (
    <Stack.Navigator
      initialRouteName="Feed"
      headerMode="screen"
      screenOptions={{
        headerTitle: state.group.name,
        header: ({ scene, previous, navigation }) => (
          <Header scene={scene} previous={previous} navigation={navigation} />
        ),
      }}
    >
      <Stack.Screen
        name="Feed"
        component={FeedContent}
      />
      <Stack.Screen
        name="Event"
        component={EventView}
      />
      <Stack.Screen
        name="VoteTime"
        component={VoteTime}
      />
      <Stack.Screen
        name="AddLocation"
        component={AddLocation}
      />
      <Stack.Screen
        name="VoteLocation"
        component={VoteLocation}
      />

    </Stack.Navigator>
  );
};

export default Feed;

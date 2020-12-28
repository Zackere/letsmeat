import { MaterialCommunityIcons } from '@expo/vector-icons';
import { createMaterialBottomTabNavigator } from '@react-navigation/material-bottom-tabs';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import React from 'react';
import { getFocusedRouteNameFromRoute } from '@react-navigation/native';
// import { NavigationState } from 'react-navigation';
import Feed from '../Feed';
import NewEvent from '../NewEvent';

import { Settings } from '../Settings';

const Tab = createBottomTabNavigator();

const makeIcon = (name) => ({ focused, color, size }) => (
  <MaterialCommunityIcons
    name={name}
    size={focused ? 25 : 20}
    color={color}
  />
);

const getActiveRouteState = function (route) {
  if (!route.routes || route.routes.length === 0 || route.index >= route.routes.length) {
    return route;
  }

  const childActiveRoute = route.routes[route.index];
  return getActiveRouteState(childActiveRoute);
};

export const BottomTabs = ({ navigation, route }) => {
  return (
    <Tab.Navigator
      initialRouteName="Feed"
    >
      <Tab.Screen
        name="Feed"
        component={Feed}
        options={{
          tabBarVisible: true,
          tabBarIcon: makeIcon('home-account'),
        }}
      />
      <Tab.Screen
        name="New Event"
        component={NewEvent}
        options={{
          tabBarIcon: makeIcon('calendar-plus'),
        }}
      />
      <Tab.Screen
        name="Group Info"
        component={Settings}
        options={{
          tabBarIcon: makeIcon('account-group'),
        }}
      />
    </Tab.Navigator>
  );
};
export default BottomTabs;

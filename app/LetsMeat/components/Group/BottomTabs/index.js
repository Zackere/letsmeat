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

export const BottomTabs = ({ navigation, route }) => (
  <Tab.Navigator
    initialRouteName="Feed"
    tabBarOptions={{
      style: {
        position: 'absolute',
        backgroundColor: 'rgba(200, 200, 200, 0.7)',
        left: 0,
        bottom: 0,
        right: 0,
        elevation: 0,
        borderTopWidth: 0
      }
    }}
  >
    <Tab.Screen
      name="Feed"
      component={Feed}
      options={{
        tabBarVisible: true,
        tabBarIcon: makeIcon('home-account'),
      }}
    />
    {/* <Tab.Screen
      name="New Event"
      component={NewEvent}
      options={{
        tabBarIcon: makeIcon('calendar-plus'),
      }}
    /> */}
    <Tab.Screen
      name="Group Info"
      component={Settings}
      options={{
        tabBarIcon: makeIcon('account-group'),
      }}
    />
  </Tab.Navigator>
);

export default BottomTabs;

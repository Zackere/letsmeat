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
      activeTintColor: 'rgba(240, 240, 240, 1)',
      inactiveTintColor: 'rgba(30, 30, 30, 1)',
      labelStyle: {
        fontSize: 12
      },
      style: {
        position: 'absolute',
        backgroundColor: 'rgba(160, 160, 160, 0.7)',
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

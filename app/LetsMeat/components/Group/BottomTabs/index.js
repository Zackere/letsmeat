import React from 'react';
import { createMaterialBottomTabNavigator } from '@react-navigation/material-bottom-tabs';

import { MaterialCommunityIcons } from '@expo/vector-icons';
import Feed from '../Feed';
import NewEvent from '../NewEvent';
import { Settings } from '../Settings';
import { DefaultTheme } from '@react-navigation/native';
import { useTheme } from '@react-navigation/native';


const Tab = createMaterialBottomTabNavigator();

const makeIcon = (name) => ({ focused, color, size }) => (
  <MaterialCommunityIcons
    name={name}
    size={focused ? 25 : 20}
    color={color}
  />
);

export const BottomTabs = ({ navigation }) => {
  const theme = useTheme();
  return (
    <Tab.Navigator
      initialRouteName="Feed"
      // screenOptions={{
      //   header: ({ scene, previous, navigation }) => (
      //     <Header scene={scene} previous={previous} navigation={navigation} />
      //   ),
      // }}
    >
      <Tab.Screen
        name="Feed"
        component={Feed}
        options={{
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

import React from 'react';
import { TouchableOpacity, Text } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import { Appbar, Avatar } from 'react-native-paper';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { useTheme } from '@react-navigation/native';
import Feed from '../Group/Feed';
import { Settings as GroupSettings } from '../Group/Settings';
import NewEvent from '../Group/NewEvent';


export const Main = () => {
  const Stack = createStackNavigator();
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
        name="New Event"
        component={NewEvent}
        options={{ headerTitle: 'New Event' }}
      />
      <Stack.Screen
        name="Feed"
        component={Feed}
        options={{ headerTitle: 'Feed' }}
      />
      <Stack.Screen
        name="Group Settings"
        component={GroupSettings}
        options={{ headerTitle: 'Group Settings' }}
      />
    </Stack.Navigator>
  );
};

export default Main;

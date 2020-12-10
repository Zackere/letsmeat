import React from 'react';
import { Text, View } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import { ScrollView } from 'react-native-gesture-handler';
import { Header } from './Header';
import { Groups } from './Select';
import { Groups } from './create';

const Stack = createStackNavigator();

const NewEvent = () => (
  <Stack.Navigator
    initialRouteName="Groups"
    headerMode='screen'
    headerTitle="Select Group"
    screenOptions={{
      header: ({ scene, previous, navigation }) => (
        <Header scene={scene} previous={previous} navigation={navigation} />
      ),
    }}
  >
    <Stack.Screen
      name="Groups"
      component={Groups}
      options={{ headerTitle: 'Available groups' }}
    />
    <Stack.Screen
      name="CreateGroup"
      component={Groups}
      options={{ headerTitle: 'Create a group' }}
    />
  </Stack.Navigator>
);

export default NewEvent;

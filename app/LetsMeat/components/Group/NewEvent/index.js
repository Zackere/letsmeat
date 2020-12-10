import React from 'react';
import { Text, View } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import { Header } from '../Header';
import { ScrollView } from 'react-native-gesture-handler';

const Stack = createStackNavigator();

const NewEvent = () => (
  <Stack.Navigator
    initialRouteName="NewEvent"
    headerMode="float"
    headerTitle="XD"
    screenOptions={{
      header: ({ scene, previous, navigation }) => (
        <Header scene={scene} previous={previous} navigation={navigation} />
      ),
    }}
  >
    <Stack.Screen
      name="NewEvent"
      component={() => (<Text>XD</Text>)}
      options={{ headerTitle: 'Znajomi' }}
    />
  </Stack.Navigator>
);

export default NewEvent;

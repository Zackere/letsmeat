import React from 'react';
import { Text, View } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import { ScrollView } from 'react-native-gesture-handler';
import { Header } from './Header';
import { Groups as SelectGroup } from './Select';
import { Create } from './Create';

const Stack = createStackNavigator();

const Groups = () => (
  <Stack.Navigator
    initialRouteName="SelectGroup"
    headerMode="screen"
    screenOptions={{
      header: ({ scene, previous, navigation }) => (
        <Header scene={scene} previous={previous} navigation={navigation} />
      ),
    }}
  >
    <Stack.Screen
      name="SelectGroup"
      component={SelectGroup}
      options={{ headerTitle: 'Available groups' }}
    />
    <Stack.Screen
      name="CreateGroup"
      component={Create}
      options={{ headerTitle: 'Create a group' }}
    />
  </Stack.Navigator>
);

export default Groups;

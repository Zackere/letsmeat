import { createStackNavigator } from '@react-navigation/stack';
import React from 'react';
import { StyleSheet } from 'react-native';
import { Header } from '../Header';
import NotificationsScreen from './notificationsScreen';

const Stack = createStackNavigator();

const Notifications = () => (
  <Stack.Navigator
    initialRouteName="NotificationsContent"
    headerMode="screen"
    screenOptions={{
      header: ({ scene, previous, navigation }) => (
        <Header scene={scene} previous={previous} navigation={navigation} />
      ),
    }}
  >
    <Stack.Screen
      name="NotificationsContent"
      component={NotificationsScreen}
      options={{ headerTitle: 'Notifications' }}
    />
  </Stack.Navigator>
);

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  title: {
    fontSize: 50,
    margin: 50
  },
  card: {
    marginTop: 5,
    margin: 20
  },
  slider: {
    margin: 10
  }
});
export default Notifications;

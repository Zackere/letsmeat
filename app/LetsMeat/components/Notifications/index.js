import React from 'react';
import { StyleSheet } from 'react-native';
import { Paragraph, Surface } from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { Header } from '../Header';

const NotificationsContent = ({ navigation }) => {
  const n = navigation;
  return (
    <Surface style={styles.container}>
      <Paragraph>
        To be implemented
      </Paragraph>
    </Surface>
  );
};

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
      component={NotificationsContent}
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

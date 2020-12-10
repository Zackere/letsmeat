import React from 'react';
import { Text, View } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import { Header } from '../Header';
import { ScrollView } from 'react-native-gesture-handler';

const Stack = createStackNavigator();

const FeedScroll = () => {
  return (
    <ScrollView>

    </ScrollView>
  )
}

const Feed = () => (
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
      name="Feed"
      component={FeedScroll}
      options={{ headerTitle: 'Twitter' }}
    />
  </Stack.Navigator>
);

export default Feed;

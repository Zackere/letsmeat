import React, { useContext } from 'react';
import { Text, View } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import { ScrollView, TouchableHighlight } from 'react-native-gesture-handler';
import { Header } from '../Header';
import { store } from '../../Store';
import { getGroupInfo } from '../../Requests';

const Stack = createStackNavigator();

const FeedScroll = () => {
  const { state, dispatch } = useContext(store);
  return (
    <TouchableHighlight onPress={() =>
    {
      console.log(state)
    }
    }>
      <Text>nanananana</Text>
    </TouchableHighlight>
  );
};

const loadGroup = (state, dispatch) => {
  if (state.group.id) {
    return getGroupInfo({ state }, state.group.id).then((group) => dispatch({ action: 'SET_GROUP', payload: group }));
  }
  return Promise.reject(new Error('No group ID in state'));
};

const Feed = () => {
  const { state, dispatch } = useContext(store);

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
        name="Feed"
        component={FeedScroll}
        options={{ headerTitle: state.group.name }}
      />
    </Stack.Navigator>
  );
};
export default Feed;

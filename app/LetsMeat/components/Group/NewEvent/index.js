import React, { useContext, useState } from 'react';
import { Text, View, StyleSheet } from 'react-native';
import { Surface, TextInput } from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { ScrollView } from 'react-native-gesture-handler';
import { Header } from '../Header';
import { store } from '../../Store';
import { getGroupInfo } from '../../Requests';

const Stack = createStackNavigator();

const NewEventContent = ({ navigation }) => {
  const [eventName, setName] = useState('');
  const [eventDeadline, setDeadline] = useState('');

  return (
    <Surface style={styles.container}>
      <TextInput
        style={styles.textInput}
        mode="outlined"
        label="Event Name"
        onChangeText={setName}
        value={eventName}
      />

    </Surface>
  );
};

const NewEvent = () => {
  const { state, dispatch } = useContext(store);
  return (
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
        component={NewEventContent}
        options={{ headerTitle: state.group.name }}
      />
    </Stack.Navigator>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  textInput: {
    margin: 20
  }
});

export default NewEvent;

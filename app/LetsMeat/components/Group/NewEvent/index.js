import React, { useContext, useState, useEffect } from 'react';
import { Text, View, StyleSheet } from 'react-native';
import {
  Surface, TextInput, Button, Card, Paragraph
} from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { ScrollView } from 'react-native-gesture-handler';
import DateTimePicker from '@react-native-community/datetimepicker';
import { Header } from '../Header';
import { store } from '../../Store';
import { createEvent } from '../../Requests';

const Stack = createStackNavigator();

const NewEventContent = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  const [eventName, setName] = useState('');
  const [eventDeadline, setDeadline] = useState(null);
  const [show, setShow] = useState(false);
  const [mode, setMode] = useState('date');
  const [timeLeftUntilDeadline, setTimeLeft] = useState('');
  const [timeUpdater, setTimeUpdater] = useState(null);
  const updateTimeLeft = () => {
    console.log('timer tick tock')
    if (!eventDeadline) return;
    const msLeft = (eventDeadline - new Date());
    const days = Math.floor(msLeft / (1000 * 60 * 60 * 24));
    const hours = Math.floor(msLeft / (1000 * 60 * 60)) - 24 * days;
    const minutes = Math.floor(msLeft / (1000 * 60)) - 24 * 60 * days - 60 * hours;
    const seconds = Math.floor(msLeft / 1000) - 24 * 60 * 60 * days - 60 * 60 * hours - 60 * minutes;
    setTimeLeft(`${days}d ${hours}h ${minutes}m ${seconds}s`);
  };

  useEffect(() => () => {
    console.log('cleanup')
    if (timeUpdater) clearInterval(timeUpdater);
  }, []);

  const now = new Date();

  const onSetDate = (event, date) => {
    clearInterval(timeUpdater);
    setShow(false);
    if (event.type == 'dismissed') {
      setMode('date');
      return;
    }
    setDeadline(new Date(event.nativeEvent.timestamp));
    if (mode === 'date') {
      setMode('time');
      setShow(true);
    }
    if (mode === 'time') {
      setTimeUpdater(setInterval(updateTimeLeft, 1000));
      setMode('date');
    }
  };

  return (
    <Surface style={styles.container}>
      <TextInput
        style={styles.textInput}
        mode="outlined"
        label="Event Name"
        onChangeText={setName}
        value={eventName}
      />
      <Card
        style={styles.emptyCard}
        onPress={() => {
          setShow(true);
          setDeadline(new Date());
        }}
      >
        <Card.Title title={eventDeadline ? eventDeadline.toString() : 'Click to set voting deadline'} />
        <Card.Content>
          <Paragraph>
            {timeLeftUntilDeadline}
          </Paragraph>
        </Card.Content>
      </Card>
      <Button onPress={() => {
        createEvent({ state }, state.group.id, eventName, eventDeadline).then(navigation.navigate('Feed'));
      }}
      >
        Create Event
      </Button>
      {/* {`Voting Deadline : \n${eventDeadline || 'click to select'}` }
      <Button
        style={styles.dateButton}
        contentStyle={styles.dateButton}
        onPress={() => {
          setShow(true);
          setDeadline(new Date());
        }}
      /> */}
      {show && (
      <DateTimePicker
        onChange={(event) => {
          onSetDate(event);
        }}
        value={eventDeadline}
        minimumDate={now}
        mode={mode}
        display="spinner"
      />
      )}
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
  },
  textInputDown: {
    marginTop: '100%'
  },
  dateButton: {
    height: 100
  },
  emptyCard: {
    // width: '100%',
    margin: 25
  }
});

export default NewEvent;

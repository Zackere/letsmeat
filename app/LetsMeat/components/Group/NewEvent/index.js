import React, { useContext, useState } from 'react';
import { StyleSheet } from 'react-native';
import {
  Button, Card, Surface, TextInput, Title
} from 'react-native-paper';
import BackgroundContainer from '../../Background';
import { createEvent } from '../../Requests';
import { store } from '../../Store';
import { DateAndHourPicker, TimeCard } from '../Feed/times';

export const NewEventContent = ({ navigation }) => {
  const DEFAULT_DEADLINE = null;
  const DEFAULT_NAME = null;

  const { state, dispatch } = useContext(store);
  const [pickerVisible, setPickerVisible] = useState(false);
  const [name, setName] = useState(null);
  const [deadline, setDeadline] = useState(null);

  React.useEffect(() => {
    const unsubscribe = navigation.addListener('blur', () => {
      setDeadline(DEFAULT_DEADLINE);
      setName(DEFAULT_NAME);
    });
    return unsubscribe;
  }, [navigation]);

  const inputValid = deadline && name && deadline > new Date();

  return (
    <BackgroundContainer backgroundVariant="fireworks" loading>
      <Card style={styles.titleCard}>
        <Card.Content>
          <Title style={styles.title}>{name || 'You should supply a name for your event'}</Title>
        </Card.Content>
      </Card>
      {deadline
        ? <TimeCard time={deadline} />
        : (
          <Card style={{ margin: 10, backgroundColor: 'rgba(240, 240, 240, 0.8)' }} onPress={() => setPickerVisible(true)}>
            <Card.Title title="The event should have some deadline" />
          </Card>
        )}
      <TextInput onChangeText={setName} style={{ margin: 5 }} mode="outlined" label="Event name" />
      <Button
        style={styles.button}
        mode="contained"
        onPress={() => {
          setPickerVisible(true);
        }}
      >
        Set event deadline
      </Button>
      <Button
        style={styles.button}
        mode="contained"
        disabled={!inputValid}
        onPress={() => {
          createEvent({ state }, state.group.id, name, deadline)
            .then((r) => dispatch({ type: 'ADD_EVENT', event: r }))
            .then(() => navigation.goBack());
        }}
      >
        Create Event
      </Button>
      <DateAndHourPicker
        visible={pickerVisible}
        setVisible={setPickerVisible}
        setPickerVisible={setPickerVisible}
        minimumDate={new Date()}
        setValue={setDeadline}
      />
    </BackgroundContainer>
  );
};

const styles = StyleSheet.create({
  title: {
    color: 'black',
    fontSize: 30,
    margin: 20,
    textAlign: 'center'
  },
  titleCard: {
    margin: 10,
    backgroundColor: 'rgba(240, 240, 240, 0.8)'
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
    margin: 25
  },
  button: {
    margin: 10
  }
});

const NewEvent = NewEventContent;

export default NewEvent;

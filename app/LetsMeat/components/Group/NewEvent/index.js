import { createStackNavigator } from '@react-navigation/stack';
import React, { useContext, useState } from 'react';
import { StyleSheet } from 'react-native';
import {
  Button, Dialog, Portal, Surface, TextInput, Title, Card, Text
} from 'react-native-paper';
import { Header } from '../../Header';
import { createEvent } from '../../Requests';
import { store } from '../../Store';
import { DateAndHourPicker, TimeCard } from '../Feed/times';

const Stack = createStackNavigator();

const SetNameDialog = ({
  dialogVisible, setDialogVisible, onAccept, onDismiss
}) => {
  const [result, setResult] = useState('');
  const showDialog = () => setDialogVisible(true);
  const hideDialog = () => setDialogVisible(false);

  return (
    <Portal>
      <Dialog visible={dialogVisible} onDismiss={onDismiss} contentContainerStyle={styles.container}>
        <Dialog.Content>
          <TextInput
            mode="outlined"
            label="Event Name"
            onChangeText={setResult}
            value={result}
          />
        </Dialog.Content>
        <Dialog.Actions>
          <Button
            style={{ color: 'red' }}
            onPress={
            () => {
              hideDialog();
              onDismiss();
              setResult('');
            }
          }
          >
            Cancel
          </Button>
          <Button onPress={() => {
            hideDialog();
            onAccept(result);
            setResult('');
          }}
          >
            Accept
          </Button>
        </Dialog.Actions>
      </Dialog>
    </Portal>
  );
};

export const NewEventContent = ({ navigation }) => {
  const DEFAULT_DEADLINE = null;
  const DEFAULT_NAME = null;

  const { state, dispatch } = useContext(store);
  const [pickerVisible, setPickerVisible] = useState(false);
  const showPicker = () => setPickerVisible(true);
  const [dialogVisible, setDialogVisible] = useState(true);
  const [name, setName] = useState(null);
  const [deadline, setDeadline] = useState(null);
  const [justOpened, setJustOpened] = useState(false);

  const onNameDismiss = () => {
    if (justOpened) {
      showPicker();
    }
  };

  React.useEffect(() => {
    const unsubscribe = navigation.addListener('blur', () => {
      setDeadline(DEFAULT_DEADLINE);
      setName(DEFAULT_NAME);
    });
    return unsubscribe;
  }, [navigation]);

  const onNameSet = (newName) => {
    if (justOpened) {
      showPicker();
    }
    if (!newName) {
      return;
    }
    setName(newName);
  };

  const inputValid = deadline && name && deadline > new Date();

  return (
    <Surface style={styles.container}>
      <Title style={styles.eventTitle}>{name || 'You should supply a name for your event'}</Title>
      {deadline
        ? <TimeCard time={deadline} />
        : (
          <Card style={{ margin: 10 }}>
            <Card.Title title="The event should have some deadline" />
          </Card>
        )}
      <TextInput onChangeText={setName} style={{ margin: 5 }} mode="outlined" label="Event name" />
      <Button onPress={() => {
        setPickerVisible(true);
      }}
      >
        Set event deadline
      </Button>
      <Button
        disabled={!inputValid}
        onPress={() => {
          createEvent({ state }, state.group.id, name, deadline)
            // .then(() => dispatch({ type: 'ADD_EVENT', event: { name, deadline: `${deadline}` } }))
            // .then(navigation.navigate('Feed'));
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
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20,
    marginBottom: 30
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

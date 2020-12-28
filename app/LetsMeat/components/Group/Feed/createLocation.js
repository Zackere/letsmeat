import React, { useContext, useState } from 'react';
import { StyleSheet } from 'react-native';
import { Surface, TextInput, Button } from 'react-native-paper';
import { CustomLocationCard } from '../../Location';
import { store } from '../../Store';
import { createLocationCustom, updateEvent } from '../../Requests';

const CreateLocation = ({ navigation, route }) => {
  const { state } = useContext(store);
  const [name, setName] = useState('');
  const [address, setAddress] = useState('');

  const { eventId } = route.params;
  const groupId = state.group.id;

  return (
    <Surface style={styles.container}>
      <CustomLocationCard location={{ name, address }} />
      <TextInput label="Name" style={styles.input} mode="outlined" onChangeText={setName} value={name} />
      <TextInput label="Address" style={styles.input} mode="outlined" onChangeText={setAddress} value={address} />
      <Button onPress={() => {
        createLocationCustom({ state }, groupId, name, address)
          .then((location) => updateEvent({ state }, { id: eventId, custom_locations_ids: [location.id] }))
          .then(() => navigation.pop(2));
      }}
      >
        Add Location
      </Button>
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    height: '100%',
    width: '100%'
  },
  input: {
    margin: 10
  }
});

export default CreateLocation;

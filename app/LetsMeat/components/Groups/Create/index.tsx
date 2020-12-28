import React, { useState, useContext } from 'react';
import { StyleSheet } from 'react-native';
import { Button, Surface, TextInput } from 'react-native-paper';
import { store } from '../../Store';
import { createGroup } from '../../Requests';

export const Create = ({ navigation }) => {
  const [name, setName] = useState('');
  const [nameValid, setNameValid] = useState(null);
  const { state, dispatch } = useContext(store);

  const validateName = (name: string) => {
    setNameValid(name && name.length <= 256);
  };

  const setAndValidateName = (text) => {
    validateName(text);
    setName(text);
  };

  const createNewGroup = () => {
    createGroup({ state }, name).then((group) => {
      setName('');
      setNameValid(null);
      dispatch({ type: 'ADD_GROUP', group });
      navigation.navigate('SelectGroup');
    });
  };

  return (
    <Surface style={styles.container}>
      <TextInput
        style={styles.textInput}
        mode="outlined"
        label="New Group Name"
        value={name}
        error={nameValid !== null && !nameValid}
        onChangeText={setAndValidateName}
      />
      <Button
        style={styles.button}
        mode="contained"
        icon="plus"
        disabled={nameValid !== null && !nameValid}
        onPress={createNewGroup}
      >
        Create
      </Button>
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  textInput: {
    margin: 15,
  },
  button: {
    margin: 15,
    marginLeft: 'auto',
    width: 120,
  }
});

export default Create;

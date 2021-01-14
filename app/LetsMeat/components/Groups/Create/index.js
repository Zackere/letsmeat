import React, { useState, useContext } from 'react';
import { StyleSheet } from 'react-native';
import {
  ActivityIndicator, Button, Surface, TextInput
} from 'react-native-paper';
import { store } from '../../Store';
import { createGroup } from '../../Requests';
import { BackgroundContainer } from '../../Background';

export const Create = ({ navigation }) => {
  const [name, setName] = useState('');
  const [nameValid, setNameValid] = useState(null);
  const { state, dispatch } = useContext(store);
  const [creating, setCreating] = useState(false);

  const validateName = (name) => {
    setNameValid(name && name.length <= 256);
  };

  const setAndValidateName = (text) => {
    validateName(text);
    setName(text);
  };

  const createNewGroup = () => {
    setCreating(true);
    createGroup({ state, dispatch }, name).then((group) => {
      dispatch({ type: 'ADD_GROUP', group });
      navigation.goBack();
    })
      .catch(() => setCreating(false));
  };

  return (
    <BackgroundContainer backgroundVariant="office">
      <TextInput
        style={styles.textInput}
        mode="outlined"
        label="New Group Name"
        value={name}
        error={nameValid !== null && !nameValid}
        onChangeText={setAndValidateName}
      />
      {
        creating ? (
          <ActivityIndicator />
        ) : (
          <Button
            style={styles.button}
            mode="contained"
            icon="plus"
            disabled={nameValid !== null && !nameValid}
            onPress={createNewGroup}
          >
            Create
          </Button>
        )
}
    </BackgroundContainer>
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
  }
});

export default Create;

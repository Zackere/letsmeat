import React, { useContext, useState } from 'react';
import {
  StyleSheet
} from 'react-native';
import {
  Button, Card, Surface, TextInput, ToggleButton
} from 'react-native-paper';
import { addDebt, createImageDebt, updateImageDebt } from '../../Requests';
import { store } from '../../Store';
import UserCard, { UserPicker } from '../../User';

const AddDebt = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
  const { eventId, imageId, debt } = route.params;

  const [direction, setDirection] = useState('IOwe');
  const [amount, setAmount] = useState(debt ? `${debt.amount / 100}` : '');
  const [description, setDescription] = useState(debt ? `${debt.description}` : '');
  const [userPickerOpen, setUserPickerOpen] = useState(false);
  const [user, setUser] = useState(null);

  // const pressAddDebt = () => {
  //   const [from, to] = direction === 'IOwe' ? [state.user.id, user.id] : [user.id, state.user.id];
  //   addDebt({ state }, state.group.id, eventId, from, to, parseFloat(amount) * 100, description, imageId).then(() => navigation.goBack());
  // };

  const reloadDebts = () => dispatch({ type: 'SET_EVENT', payload: { ...state.event, images: [...state.event.images] } });

  const pressAddDebt = () => {
    // const [from, to] = direction === 'IOwe' ? [state.user.id, user.id] : [user.id, state.user.id];
    if (debt) {
      updateImageDebt({ state }, { ...debt, amount: parseFloat(amount) * 100, description }).then(() => {
        reloadDebts();
        navigation.goBack();
      });
    } else {
      createImageDebt({ state }, parseFloat(amount) * 100, description, imageId).then(() => {
        reloadDebts();
        navigation.goBack();
      });
    }
  };

  const buttonText = direction === 'IOwe' ? 'Select the person you owe' : 'Select the person who owes you';

  return (
    <Surface style={styles.container}>
      {/* <ToggleButton.Row style={{
        justifyContent: 'center',
        marginVertical: 10,
        flexDirection: 'row',
        alignSelf: 'center',
        borderColor: 'rgba(1, 1, 1, 0.2)',
        borderWidth: 1,
        borderRadius: 5
      }}
      >
        <Button
          mode={direction === 'IOwe' ? 'contained' : 'text'}
          onPress={() => setDirection('IOwe')}
        >
          They paid for me
        </Button>
        <Button
          mode={direction === 'TheyOwe' ? 'contained' : 'text'}
          onPress={() => setDirection('TheyOwe')}
        >
          I Paid for them
        </Button>
      </ToggleButton.Row>
      {
      user
        ? <UserCard user={user} /> : <Card />
    } */}
      {/* <Button onPress={() => setUserPickerOpen(true)}>{buttonText}</Button> */}
      <TextInput
        mode="outlined"
        label="Amount"
        style={{ margin: 10 }}
        value={`${amount}`}
        onChangeText={setAmount}
        placeholder="10,25"
        // right={<Icon name="currency-usd" size={25} color="black" />}
        keyboardType="numeric"
      />
      <TextInput
        mode="outlined"
        label="Description"
        style={{ margin: 10 }}
        value={description}
        onChangeText={setDescription}
        placeholder="Those two wonderful hot-dogs :)"
      />
      <Button onPress={pressAddDebt}>Add Debt</Button>
      {/* <UserPicker
        userIds={state.group.users.map((u) => u.id).filter((id) => id !== state.user.id)}
        dialogVisible={userPickerOpen}
        onDismiss={() => setUserPickerOpen(false)}
        setUser={setUser}
      /> */}
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    height: '100%',
    width: '100%'
  },
  searchbar: {
    margin: 5
  },
  searchResult: {
    margin: 5
  },
  selectedUserContainer: {
    margin: 5
  },
  fab: {
    margin: 10, right: 0, position: 'absolute', bottom: 0
  }
});

export default AddDebt;

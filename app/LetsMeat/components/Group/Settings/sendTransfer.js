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
import { formatAmount, parseAmount } from '../../../helpers/money';
import { BackgroundContainer } from '../../Background';

export const SendTransfer = ({ navigation, route }) => {
  const { state } = useContext(store);
  const {
    user
  } = route.params;

  const [amount, setAmount] = useState(route.params.amount ? formatAmount(route.params.amount) : '');
  const [description, setDescription] = useState('');

  const pressConfirm = () => {
    addDebt({ state }, state.group.id, null, state.user.id, user.id, parseFloat(amount), description, null, 1);
  };

  return (
    <BackgroundContainer>

      {user ? (
        <UserCard
          user={user}
        />
      ) : null}
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
      <Button mode="outlined" onPress={pressConfirm}>Send request for transfer confirmation</Button>
    </BackgroundContainer>
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

export default SendTransfer;

import React, { useContext, useState } from 'react';
import {
  Button, TextInput
} from 'react-native-paper';
import { MAX_DEBT_DESCRIPTION_LENGTH } from '../../../constants';
import { formatAmount, isAmountValid, parseAmount } from '../../../helpers/money';
import { BackgroundContainer } from '../../Background';
import { addDebt } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

export const SendTransfer = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
  const {
    user
  } = route.params;

  const [amount, setAmount] = useState(route.params.amount ? formatAmount(route.params.amount) : '');
  const [description, setDescription] = useState('');

  const valid = isAmountValid(amount) && description.length <= MAX_DEBT_DESCRIPTION_LENGTH;

  const pressConfirm = () => {
    if (!valid) return;
    addDebt({ state, dispatch },
      state.group.id, null, state.user.id, user.id, parseAmount(amount), description, null, 1);
    navigation.goBack();
  };

  return (
    <BackgroundContainer backgroundVariant="money">
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
        keyboardType="numeric"
        error={amount && !isAmountValid(amount)}
      />
      <TextInput
        mode="outlined"
        label="Description"
        style={{ margin: 10 }}
        value={description}
        onChangeText={(text) => setDescription(text.slice(0, MAX_DEBT_DESCRIPTION_LENGTH))}
        placeholder="Thank you for all the fish"
      />
      <Button
        disabled={!valid}
        mode="contained"
        style={{ margin: 10 }}
        onPress={pressConfirm}
      >
        Send request for transfer confirmation
      </Button>
    </BackgroundContainer>
  );
};

export default SendTransfer;

import React, { useContext, useState } from 'react';
import {
  StyleSheet
} from 'react-native';
import {
  Button, TextInput
} from 'react-native-paper';
import { MAX_DEBT_DESCRIPTION_LENGTH } from '../../../constants';
import { isAmountValid, parseAmount } from '../../../helpers/money';
import { BackgroundContainer } from '../../Background';
import { createImageDebt, updateImageDebt } from '../../Requests';
import { store } from '../../Store';

const AddDebt = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
  const { imageId, debt } = route.params;

  const [amount, setAmount] = useState(debt ? `${debt.amount / 100}` : '');
  const [description, setDescription] = useState(debt ? `${debt.description}` : '');

  const valid = isAmountValid(amount) && description.length <= MAX_DEBT_DESCRIPTION_LENGTH;

  const reloadDebts = () => dispatch({ type: 'SET_EVENT', payload: { ...state.event, images: [...state.event.images] } });

  const pressAddDebt = () => {
    if (debt) {
      updateImageDebt({ state, dispatch }, { ...debt, amount: parseAmount(amount), description }).then(() => {
        reloadDebts();
        navigation.goBack();
      });
    } else {
      createImageDebt({ state, dispatch }, parseAmount(amount), description, imageId).then(() => {
        reloadDebts();
        navigation.goBack();
      });
    }
  };

  return (
    <BackgroundContainer backgroundVariant="money">
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
        mode="contained"
        style={styles.button}
        onPress={pressAddDebt}
        disabled={!valid}
      >
        Add Debt
      </Button>
    </BackgroundContainer>
  );
};

const styles = StyleSheet.create({
  button: {
    margin: 10
  }
});

export default AddDebt;

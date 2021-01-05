import {
  GoogleSignin, GoogleSigninButton,

  statusCodes
} from '@react-native-community/google-signin';
import React, { useContext, useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { appendAPIToken, appendUserID } from '../Requests';
import { store } from '../Store';

function SignInScreen() {
  const { dispatch } = useContext(store);
  const [signingIn, setSigningIn] = useState(false);

  const setUser = (userInfo) => appendAPIToken(userInfo)
    .then(appendUserID)
    .then((userInfo) => {
      console.log(userInfo);
      dispatch({ type: 'SET_USER', payload: userInfo });
    });

  const signIn = async () => {
    setSigningIn(true);
    try {
      await GoogleSignin.hasPlayServices();
      const userInfo = await GoogleSignin.signIn();
      setUser(userInfo);
    } catch (error) {
      console.log('Message', error.message);
      if (error.code === statusCodes.SIGN_IN_CANCELLED) {
        console.log('User Cancelled the Login Flow');
      } else if (error.code === statusCodes.IN_PROGRESS) {
        console.log('Signing In');
      } else if (error.code === statusCodes.PLAY_SERVICES_NOT_AVAILABLE) {
        console.log('Play Services Not Available or Outdated');
      } else {
        console.log('Some Other Error Happened');
      }
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Let&apos;s meat</Text>
      <GoogleSigninButton
        style={{ width: 200, height: 50 }}
        size={GoogleSigninButton.Size.Wide}
        color={GoogleSigninButton.Color.Dark}
        onPress={signIn}
        disabled={signingIn}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    flexDirection: 'column',
    alignItems: 'center',
  },
  title: {
    fontSize: 50,
    margin: 50
  },
});

export default SignInScreen;

import React, { useContext, useEffect } from 'react';
import {
  GoogleSigninButton,
  GoogleSignin,
  statusCodes
} from '@react-native-community/google-signin';
import { View, StyleSheet, Text } from 'react-native';
import * as Progress from 'react-native-progress';

function SplashScreen() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Let&apos;s meat</Text>
      <Progress.Bar size={30} indeterminate={true} />
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
  }
});

export default SplashScreen;

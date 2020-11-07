import React, { useEffect, useContext } from 'react';
import {
  StyleSheet, View, Text, LogBox
} from 'react-native';
import { TouchableHighlight } from 'react-native-gesture-handler';
import { store } from '../Store';
import {
  GoogleSignin,
  GoogleSigninButton,
  statusCodes,
} from '@react-native-community/google-signin';

function Main({ navigation }) {
  const { state, dispatch } = useContext(store);

  return (
    <>
      <View style={styles.container}>
        <TouchableHighlight
          onPress={() => {
            navigation.openDrawer();
          }}
        >
          <View style={styles.button}>
            <Text>Open Drawer</Text>
          </View>
        </TouchableHighlight>
      </View>
    </>
  );
}

const styles = StyleSheet.create({
  button: {
    backgroundColor: '#DDDDDD',
    alignItems: 'center',
    padding: 10,
  },
  container: {
    flex: 1,
    paddingHorizontal: 10,
    justifyContent: 'center',
  },
});

export default Main;

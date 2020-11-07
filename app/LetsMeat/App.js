/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow strict-local
 */
import 'react-native-gesture-handler';
import React, { useContext, useEffect } from 'react';
import {
  StyleSheet,
} from 'react-native';

import { Provider as PaperProvider } from 'react-native-paper';

import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { createDrawerNavigator } from '@react-navigation/drawer';
import {
  GoogleSignin
} from '@react-native-community/google-signin';
import Main from './components/Main';
import DrawerContent from './components/DrawerContent';

import SignInScreen from './components/SignIn';
import SplashScreen from './components/SplashScreen';
import { StateProvider, store } from './components/Store';

const Drawer = createDrawerNavigator();
const Stack = createStackNavigator();

const Navigation = () => {
  const { state, dispatch } = useContext(store);

  useEffect(() => {
    GoogleSignin.configure({
      webClientId: '790166575655-222h23mpv6h7n7jhjkac6gj220e5kevt.apps.googleusercontent.com'
    });
    GoogleSignin.isSignedIn().then((signedIn) => {
      if (signedIn) {
        GoogleSignin.getCurrentUser().then((userInfo) => {
          dispatch({ type: 'SET_USER', payload: userInfo });
          dispatch({ type: 'SET_LOADED' });
        });
      }
    });
  }, []);

  let mainComponent = null;
  if (state.loading) {
    mainComponent = (
      <Stack.Navigator>
        <Stack.Screen name="SplashScreen" component={SplashScreen} options={{ headerShown: false }} />
      </Stack.Navigator>
    );
  } else if (state.user.signedIn) {
    mainComponent = (
      <Drawer.Navigator
        drawerContent={() => <DrawerContent />}
      >
        <Drawer.Screen name="Home" component={Main} />
      </Drawer.Navigator>
    );
  } else {
    mainComponent = (
      <Stack.Navigator>
        <Stack.Screen name="SignIn" component={SignInScreen} options={{ headerShown: false }} />
      </Stack.Navigator>
    );
  }

  return (
    <PaperProvider>
      <NavigationContainer>
        {mainComponent}
      </NavigationContainer>
    </PaperProvider>
  );
};

const App = () => (
  <StateProvider>
    <Navigation />
  </StateProvider>
);

export default App;

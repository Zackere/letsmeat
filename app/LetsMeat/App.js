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

import { DefaultTheme as PaperDefaultTheme, Provider as PaperProvider } from 'react-native-paper';

import { DefaultTheme as NavigationDefaultTheme, NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { createDrawerNavigator } from '@react-navigation/drawer';
import {
  GoogleSignin
} from '@react-native-community/google-signin';
// import Main from './components/Main';
import BottomTabs from './components/Group/BottomTabs';
import DrawerContent from './components/DrawerContent';
// import CreateGroup from './components/Groups/CreateGroup';
import SignInScreen from './components/SignIn';
import SplashScreen from './components/SplashScreen';
import { StateProvider, store } from './components/Store';
import Groups from './components/Groups';
import { appendAPIToken, appendUserID } from './components/Requests';

const Drawer = createDrawerNavigator();
const Stack = createStackNavigator();

const Navigation = () => {
  const { state, dispatch } = useContext(store);
  console.log('I pass thru app.js');
  useEffect(() => {
    GoogleSignin.configure({
      webClientId: '790166575655-222h23mpv6h7n7jhjkac6gj220e5kevt.apps.googleusercontent.com'
    });
    GoogleSignin.isSignedIn().then((signedIn) => {
      if (signedIn) {
        console.log('yeah, signed in');
        GoogleSignin.getCurrentUser()
          .then(appendAPIToken)
          .then(appendUserID)
          .then((userInfo) => {
            dispatch({ type: 'SET_USER', payload: userInfo });
            dispatch({ type: 'SET_LOADED' });
          })
          .catch(() => {
            console.log('logging out');
            dispatch({ type: 'LOGOUT' });
            GoogleSignin.signOut();
          }).finally(() => dispatch({type: 'SET_LOADED'}));
      } else {
        dispatch({ type: 'SET_LOADED' });
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
        drawerContent={(props) => <DrawerContent navigation={props.navigation} />}
      >
        <Drawer.Screen name="Home" component={BottomTabs} />
        <Drawer.Screen name="Groups" component={Groups} />
        {/* <Drawer.Screen name="CreateGroup" component={CreateGroup} /> */}
      </Drawer.Navigator>
    );
  } else {
    mainComponent = (
      <Stack.Navigator>
        <Stack.Screen name="SignIn" component={SignInScreen} options={{ headerShown: false }} />
      </Stack.Navigator>
    );
  }

  const mergedTheme = {
    ...PaperDefaultTheme,
    ...NavigationDefaultTheme,
    colors: {
      ...PaperDefaultTheme.colors,
      ...NavigationDefaultTheme.colors,
    },
  };

  return (
    <PaperProvider theme={mergedTheme}>
      <NavigationContainer theme={mergedTheme}>
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

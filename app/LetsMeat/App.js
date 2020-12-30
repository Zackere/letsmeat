/**
 * @format
 * @flow strict-local
 */
import React, { useContext, useEffect } from 'react';
import {
  GoogleSignin
} from '@react-native-community/google-signin';
import { createDrawerNavigator } from '@react-navigation/drawer';
import { DefaultTheme as NavigationDefaultTheme, NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import 'react-native-gesture-handler';
import { DefaultTheme as PaperDefaultTheme, Provider as PaperProvider } from 'react-native-paper';
import DrawerContent from './components/DrawerContent';
import BottomTabs from './components/Group/BottomTabs';
import Groups from './components/Groups';
import Preferences from './components/Preferences';
import { appendAPIToken, appendUserID } from './components/Requests';
import SignInScreen from './components/SignIn';
import SplashScreen from './components/SplashScreen';
import { StateProvider, store } from './components/Store';
import Notifications from './components/Notifications';
import Header from './components/Header';

const Drawer = createDrawerNavigator();
const Stack = createStackNavigator();
const PERSISTENCE_KEY = 'NAVIGATION_STATE';

const Navigation = () => {
  const { state, dispatch } = useContext(store);
  useEffect(() => {
    GoogleSignin.configure({
      webClientId: '790166575655-222h23mpv6h7n7jhjkac6gj220e5kevt.apps.googleusercontent.com'
    });
    GoogleSignin.isSignedIn().then((signedIn) => {
      if (signedIn) {
        GoogleSignin.getCurrentUser()
          .then(appendAPIToken)
          .then(appendUserID)
          .then((userInfo) => {
            dispatch({ type: 'SET_USER', payload: userInfo });
            dispatch({ type: 'SET_LOADED' });
          })
          .catch(() => {
            dispatch({ type: 'LOGOUT' });
            GoogleSignin.signOut();
          })
          .finally(() => dispatch({ type: 'SET_LOADED' }));
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
        initialRouteName={state.group.id ? 'Home' : 'Groups'}
        drawerContent={(props) => (
          <DrawerContent navigation={props.navigation} />
        )}
        edgeWidth={0}
      >
        <Drawer.Screen name="Home" component={BottomTabs} />
        <Drawer.Screen name="Groups" component={Groups} />
        <Drawer.Screen name="Preferences" component={Preferences} />
        <Drawer.Screen name="Notifications" component={Notifications} />
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

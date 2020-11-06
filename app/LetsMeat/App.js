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
  Colors,
} from 'react-native/Libraries/NewAppScreen';
import {
  GoogleSignin,
  statusCodes
} from '@react-native-community/google-signin';
import Main from './components/Main';
import DrawerContent from './components/DrawerContent';

import SignInScreen from './components/SignIn';
import { StateProvider, store } from './components/Store';

const Drawer = createDrawerNavigator();
const Stack = createStackNavigator();

const Navigation = () => {
  const { state } = useContext(store);

  return (
    <PaperProvider>
      <NavigationContainer>
        {state.user.signedIn ? (
          <Drawer.Navigator
            drawerContent={() => <DrawerContent />}
          >
            <Drawer.Screen name="Home" component={Main} />
          </Drawer.Navigator>
        ) : (
          <Stack.Navigator>
            <Stack.Screen name="SignIn" component={SignInScreen} options={{ headerShown: false }} />
          </Stack.Navigator>
        )}
      </NavigationContainer>
    </PaperProvider>
  );
};

const App = () => {
  useEffect(() => {
    GoogleSignin.configure({
      webClientId: '790166575655-222h23mpv6h7n7jhjkac6gj220e5kevt.apps.googleusercontent.com'
    });
  }, []);

  return (
    <StateProvider>
      <Navigation />
    </StateProvider>
  );
};

// const styles = StyleSheet.create({
//   scrollView: {
//     backgroundColor: Colors.lighter,
//   },
//   engine: {
//     position: 'absolute',
//     right: 0,
//   },
//   body: {
//     backgroundColor: Colors.white,
//   },
//   sectionContainer: {
//     marginTop: 32,
//     paddingHorizontal: 24,
//   },
//   sectionTitle: {
//     fontSize: 24,
//     fontWeight: '600',
//     color: Colors.black,
//   },
//   sectionDescription: {
//     marginTop: 8,
//     fontSize: 18,
//     fontWeight: '400',
//     color: Colors.dark,
//   },
//   highlight: {
//     fontWeight: '700',
//   },
//   footer: {
//     color: Colors.dark,
//     fontSize: 12,
//     fontWeight: '600',
//     padding: 4,
//     paddingRight: 12,
//     textAlign: 'right',
//   },
// });

export default App;

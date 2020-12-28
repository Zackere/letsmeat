/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow strict-local
 */
import { createDrawerNavigator } from '@react-navigation/drawer';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import React, { useContext } from 'react';
import {
  StyleSheet
} from 'react-native';
import 'react-native-gesture-handler';
import { Provider as PaperProvider } from 'react-native-paper';
import {
  Colors
} from 'react-native/Libraries/NewAppScreen';
import DrawerContent from '../DrawerContent';
// import { Main } from '../Main';
import SignInScreen from '../SignIn';
import { store } from '../Store';

const Drawer = createDrawerNavigator();
const Stack = createStackNavigator();

const Application: () => React$Node = () => {
  const { state, dispatch } = useContext(store);

  return (
    <PaperProvider>
      <NavigationContainer>
        {state.user.signedIn ? (
          <Drawer.Navigator
            drawerContent={(props) => <DrawerContent {...props} />}
          >
            <Drawer.Screen name="Home" component={Main} />
          </Drawer.Navigator>
        ) : (
          <Stack.Navigator>
            <Stack.Screen name="SignIn" component={SignInScreen} />
          </Stack.Navigator>
        )}
      </NavigationContainer>
    </PaperProvider>
  );
};

const styles = StyleSheet.create({
  scrollView: {
    backgroundColor: Colors.lighter,
  },
  engine: {
    position: 'absolute',
    right: 0,
  },
  body: {
    backgroundColor: Colors.white,
  },
  sectionContainer: {
    marginTop: 32,
    paddingHorizontal: 24,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: '600',
    color: Colors.black,
  },
  sectionDescription: {
    marginTop: 8,
    fontSize: 18,
    fontWeight: '400',
    color: Colors.dark,
  },
  highlight: {
    fontWeight: '700',
  },
  footer: {
    color: Colors.dark,
    fontSize: 12,
    fontWeight: '600',
    padding: 4,
    paddingRight: 12,
    textAlign: 'right',
  },
});

export default Application;
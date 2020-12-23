import React, { useContext, useEffect, useState } from 'react';
import {
  View, StyleSheet, Text
} from 'react-native';
import { Surface, Card, Button } from 'react-native-paper';
import Slider from '@react-native-community/slider';
import { createStackNavigator } from '@react-navigation/stack';
import { store } from '../Store';
import UserCard from '../User';
import { Header } from '../Header';

const PrefSetter = ({
  prefName, displayName, setPrefs, prefs
}) => (
  <Card style={styles.card} elevation={0}>
    <Card.Title title={displayName} />
    <Slider
      style={styles.slider}
      minimumValue={0}
      maximumValue={100}
      value={prefs[prefName]}
      onSlidingComplete={(v) => {
        setPrefs({ ...prefs, [prefName]: parseInt(v) });
      }}
    />
  </Card>
);

const PreferencesContent = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  useEffect(() => { console.log(<UserCard user={state.user} />); }, [state.user]);
  const [prefs, setPrefs] = useState(state.user.prefs);

  React.useLayoutEffect(() => {
    navigation.setOptions({
      rightIcon: 'content-save',
      rightAction: () => console.log('press')
    });
  }, [navigation]);

  return (

    <Surface style={styles.container}>
      <UserCard user={{ ...state.user, picture_url: state.user.photo, prefs }} />
      <Card style={styles.card}>
        <Card.Title title="Set Preferences" />
        <PrefSetter prefName="price" displayName="Price" setPrefs={setPrefs} prefs={prefs} />
        <PrefSetter prefName="amount_of_food" displayName="Portion Size" setPrefs={setPrefs} prefs={prefs} />
        <PrefSetter prefName="waiting_time" displayName="Waiting Time" setPrefs={setPrefs} prefs={prefs} />
        <PrefSetter prefName="taste" displayName="Taste" setPrefs={setPrefs} prefs={prefs} />
      </Card>
    </Surface>
  );
};

const Stack = createStackNavigator();

const Preferences = () => (
  <Stack.Navigator
    initialRouteName="PreferencesContent"
    headerMode="screen"
    screenOptions={{
      header: ({ scene, previous, navigation }) => (
        <Header scene={scene} previous={previous} navigation={navigation} />
      ),
    }}
  >
    <Stack.Screen
      name="PreferencesContent"
      component={PreferencesContent}
      options={{ headerTitle: 'Preferences' }}
    />
  </Stack.Navigator>
);

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  title: {
    fontSize: 50,
    margin: 50
  },
  card: {
    marginTop: 5,
    margin: 20
  },
  slider: {
    margin: 10
  }
});

export default Preferences;

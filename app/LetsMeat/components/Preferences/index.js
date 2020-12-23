import React, { useContext, useEffect, useState } from 'react';
import {
  View, StyleSheet, Text
} from 'react-native';
import { Surface, Card } from 'react-native-paper';
import Slider from '@react-native-community/slider';
import { store } from '../Store';
import UserCard from '../User';

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

const Preferences = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  useEffect(() => { console.log(<UserCard user={state.user} />); }, [state.user]);
  const [prefs, setPrefs] = useState(state.user.prefs);

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

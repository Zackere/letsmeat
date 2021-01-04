import Slider from '@react-native-community/slider';
import React, {
  useContext,
  useEffect, useState
} from 'react';
import {
  StyleSheet
} from 'react-native';
import {
  Surface, Card, ActivityIndicator, Button
} from 'react-native-paper';
import LocationCard from '../../Location';
import { getLocationsInfo, rateLocation } from '../../Requests';
import { store } from '../../Store';

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

const RateLocation = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
  const { gmapsId, customId } = route.params;
  const [locationInfo, setLocationInfo] = useState(undefined);
  const [prefs, setPrefs] = useState({
    price: 50, waiting_time: 50, amount_of_food: 50, taste: 50
  });

  useEffect(() => {
    getLocationsInfo({ state }, customId ? [customId] : undefined, gmapsId ? [gmapsId] : undefined).then((info) => {
      if (gmapsId) {
        setLocationInfo({ ...info.google_maps_location_information[0], kind: 'google_maps_locations' });
      } else {
        setLocationInfo({ ...info.custom_location_infomation[0], kind: 'custom_locations' });
      }
    });
  });

  return (
    <Surface style={styles.container}>
      {
        locationInfo ? <LocationCard location={locationInfo} /> : <ActivityIndicator />
      }
      <Card style={styles.card}>
        <Card.Title title="How would you describe this place?" />
        <PrefSetter prefName="price" displayName="Low Price" setPrefs={setPrefs} prefs={prefs} />
        <PrefSetter prefName="waiting_time" displayName="Waiting Time" setPrefs={setPrefs} prefs={prefs} />
        <PrefSetter prefName="amount_of_food" displayName="Portion Size" setPrefs={setPrefs} prefs={prefs} />
        <PrefSetter prefName="taste" displayName="Taste" setPrefs={setPrefs} prefs={prefs} />
      </Card>
      <Button onPress={
        () => {
          rateLocation({ state }, prefs.taste, prefs.price, prefs.amount_of_food, prefs.waiting_time, gmapsId || undefined, customId || undefined)
            .then(() => navigation.goBack());
        }
      }
      >
        Rate
      </Button>
    </Surface>
  );
};

const styles = StyleSheet.create({
  card: {
    margin: 10
  },
  slider: {
    margin: 10
  },
  container: {
    height: '100%',
    width: '100%'
  },
  searchbar: {
    margin: 5
  },
  searchResult: {
    margin: 5
  },
  selectedUserContainer: {
    margin: 5
  },
  fab: {
    margin: 10, right: 0, position: 'absolute', bottom: 0
  }
});

export default RateLocation;
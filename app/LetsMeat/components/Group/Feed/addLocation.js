import { debounce } from 'debounce';
import React, {
  useCallback, useContext, useRef, useState,
  useEffect
} from 'react';
import {
  StyleSheet
} from 'react-native';
import { ScrollView } from 'react-native-gesture-handler';
import {
  Button, Searchbar, Surface
} from 'react-native-paper';
import { randomId } from '../../../helpers/random';
import { createLocationGoogle, searchLocation, updateEvent } from '../../Requests';
import { store } from '../../Store';
import LocationCard from '../../Location';

const combineLocations = (results) => {
  const predictions = results.google_maps_locations_predictions;
  let predictionsIndex = 0;
  const gmaps = results.google_maps_locations;
  let gmapsIndex = 0;
  const custom = results.custom_locations;
  let customIndex = 0;
  const combinedResults = [];
  for (let i = 0; i < predictions.length + gmaps.length + custom.length; i += 1) {
    if (i % 3 === 0) {
      if (customIndex >= custom.length) continue;
      combinedResults.push({ ...custom[customIndex], kind: 'custom_locations' });
      customIndex += 1;
    } else if (i % 3 === 1) {
      if (gmapsIndex >= gmaps.length) continue;
      combinedResults.push({ ...gmaps[gmapsIndex], kind: 'google_maps_locations' });
      gmapsIndex += 1;
    } else if (i % 3 === 2) {
      if (predictionsIndex >= predictions.length) continue;
      combinedResults.push({ ...predictions[predictionsIndex], kind: 'google_maps_locations_predictions' });
      predictionsIndex += 1;
    }
  }
  return combinedResults;
};

const AddLocation = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
  const { groupId, eventId } = route.params;

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const sessionToken = useState(() => randomId(32))[0];
  const persistentSearchQuery = useRef('');
  const mounted = useRef(false);

  useEffect(() => {
    mounted.current = true;
    return (() => {
      mounted.current = false;
      debouncedSearch.clear();
    });
  }, [debouncedSearch]);

  const getSearchResults = useCallback(() => {
    searchLocation({ state }, groupId, persistentSearchQuery.current, sessionToken)
      .then((results) => {
        if (!mounted.current) return;
        setSearchResults(results);
      });
  }, [groupId, sessionToken, state]);

  const debouncedSearch = useCallback(debounce(getSearchResults, 1000), []);

  const onChangeSearch = (query) => {
    setSearchQuery(query);
    persistentSearchQuery.current = query;
    if (query.length <= 3) return;
    debouncedSearch();
  };

  return (
    <Surface style={styles.container}>
      <Searchbar
        style={styles.searchbar}
        placeholder="Search"
        onChangeText={onChangeSearch}
        value={searchQuery}
      />
      <Button onPress={() => { navigation.navigate('CreateLocation', { eventId }); }}>Or add a new custom location</Button>
      <ScrollView>
        {(searchResults && Object.keys(searchResults).length) ? (combineLocations(searchResults).map((result) => (
          <LocationCard
            key={`${result.kind}${result.id}${result.place_id}${result.details && result.details.place_id}`}
            location={result}
            onPressCustom={
              () => {
                updateEvent({ state }, { id: eventId, custom_locations_ids: [result.id] })
                  .then((event) => {
                    dispatch({ type: 'SET_EVENT', payload: event });
                    navigation.goBack();
                  });
              }
              }
            onPressGMaps={
              () => {
                updateEvent({ state }, { id: eventId, google_maps_locations_ids: [result.details.place_id] })
                  .then((event) => {
                    dispatch({ type: 'SET_EVENT', payload: event });
                    navigation.goBack();
                  });
              }
              }
            onPressPrediction={
              () => {
                createLocationGoogle({ state }, result.place_id, sessionToken)
                  .then(() => updateEvent({ state }, { id: eventId, google_maps_locations_ids: [result.place_id] }))
                  .then((event) => {
                    dispatch({ type: 'SET_EVENT', payload: event });
                    navigation.goBack();
                  });
              }
              }
          />
        ))) : null}
      </ScrollView>
    </Surface>
  );
};

const styles = StyleSheet.create({
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

export default AddLocation;

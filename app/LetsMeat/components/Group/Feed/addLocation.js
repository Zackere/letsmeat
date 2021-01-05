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
import { combineLocations } from '../../../helpers/locations';
import { BackgroundContainer } from '../../Background';

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
    <BackgroundContainer>

      <Searchbar
        style={styles.searchbar}
        placeholder="Search"
        onChangeText={onChangeSearch}
        value={searchQuery}
      />
      <Button
        style={styles.orButton}
        mode="contained"
        onPress={() => { navigation.navigate('CreateLocation', { eventId }); }}
      >
        Or add a new custom location
      </Button>
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
    </BackgroundContainer>
  );
};

const styles = StyleSheet.create({
  searchbar: {
    margin: 5
  },
  searchResult: {
    margin: 5
  },
  selectedUserContainer: {
    margin: 5
  },
  orButton: {
    margin: 5,
    alignSelf: 'center'
  }
});

export default AddLocation;

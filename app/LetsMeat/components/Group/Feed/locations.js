import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, {
  useContext, useEffect, useState, useCallback
} from 'react';
import { StyleSheet, View } from 'react-native';
import {
  ActivityIndicator, Button
} from 'react-native-paper';
import { useFocusEffect } from '@react-navigation/native';
import { CustomLocationCard, GMapsCard } from '../../Location';
import { getLocationsInfo } from '../../Requests';
import { store } from '../../Store';

const Locations = ({
  // customLocations, googleLocations,
  onAdd, onVote, onRate,
  showButtons = true,
  order,
}) => {
  const { state, dispatch } = useContext(store);
  const [loading, setLoading] = useState(true);
  const [locationsOrdered, setLocationsOrdered] = useState([]);

  const customLocations = state.event.candidate_custom_locations;
  const googleLocations = state.event.candidate_google_maps_locations;

  useEffect(() => {
    setLoading(true);
    getLocationsInfo({ state, dispatch }, state.event.candidate_custom_locations, state.event.candidate_google_maps_locations)
      .then((locationsInfo) => {
        let newLocationsOrdered = [];
        if (locationsInfo) {
          if (order) {
            order.forEach(({ google_maps_location_id, custom_location_id }) => {
              let element;
              if (google_maps_location_id) {
                element = locationsInfo.google_maps_location_information.find((l) => (l.details && (l.details.place_id === google_maps_location_id)));
              } else {
                element = locationsInfo.custom_location_infomation.find((l) => l.id === custom_location_id);
              }
              newLocationsOrdered.push(element);
            });
          } else {
            newLocationsOrdered = [...locationsInfo.custom_location_infomation,
              ...locationsInfo.google_maps_location_information];
          }
        }
        setLocationsOrdered([...newLocationsOrdered]);
        setLoading(false);
      });
  }, [customLocations,
    googleLocations, state.user.tokenId, order, state.event.candidate_google_maps_locations, state.event.candidate_custom_locations]);

  return (
    loading ? <ActivityIndicator />
      : (
        <>
          <View>
            {
              locationsOrdered.map((l) => (l.details ? (
                <GMapsCard
                  location={l}
                  key={l.details.place_id}
                  onPress={() => {
                    onRate({ gmapsId: l.details.place_id });
                  }}
                />
              ) : (
                <CustomLocationCard
                  location={l}
                  key={l.id}
                  onPress={() => {
                    onRate({ customId: l.id });
                  }}
                />
              )))
            }
          </View>
          {showButtons
            ? (
              <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
                <Button
                  style={styles.addButton}
                  onPress={onAdd}
                >
                  <Icon name="plus" size={25} />
                </Button>
                <Button
                  style={styles.addButton}
                  onPress={onVote}
                  disabled={customLocations.length
                + googleLocations.length === 0}
                >
                  <Icon name="vote" size={25} />
                </Button>
              </View>
            ) : null}
        </>
      )
  );
};

const styles = StyleSheet.create({
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20
  },
  container: {
    width: '100%',
    height: '100%'
  },
  section: {
    margin: 10
  },
  card: {
    margin: 25
  },
  addButton: {
    marginBottom: 10
  },
  timeCard: {
    margin: 5,
    justifyContent: 'center'
  }
});

export default Locations;

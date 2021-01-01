import { useFocusEffect } from '@react-navigation/native';
import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet } from 'react-native';
import { ScrollView } from 'react-native-gesture-handler';
import {
  ActivityIndicator, Card, Surface, Title
} from 'react-native-paper';
import ModalButton from '../../Buttons';
import {
  getEventInfo, updateEvent, deleteEvent
} from '../../Requests';
import { store } from '../../Store';
import Creator from './creator';
import Deadline from './deadline';
import Debts from './debts';
import Locations from './locations';
import Times from './times';

const Results = ({ event }) => (
  <Locations
    showButtons={false}
    googleLocations={event.candidate_google_maps_locations}
    customLocations={event.candidate_custom_locations}
  />
);

const EventView = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
  const [eventDetails, setEventDetails] = useState(null);
  const [loading, setLoading] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const finished = state.event.deadline < new Date();

  React.useLayoutEffect(() => {
    navigation.setOptions({ tabBarVisible: false });
  }, [navigation, route]);

  const loadData = () => {
    if (deleting) return;
    getEventInfo({ state }, state.event.id).then((event) => {
      dispatch({ type: 'SET_EVENT', payload: event });
      setEventDetails(true);
    });
  };

  useEffect(loadData, [state.user.tokenId, deleting, dispatch]);

  return (
    <Surface style={styles.container}>
      <ScrollView style={styles.container}>
        { eventDetails
          ? (
            <>
              <Title style={styles.eventTitle}>{state.event.name}</Title>
              <Card elevation={0}>
                <Card.Title title={finished ? 'Final Voting Results' : 'Current Voting Results'} />
                <Results event={state.event} />
              </Card>
              <Deadline time={state.event.deadline} />
              <Creator userId={state.event.creator_id} />
              <Card style={styles.section} elevation={0}>
                <Card.Title title="Candidate Locations" />
                <Locations
                  googleLocations={state.event.candidate_google_maps_locations}
                  customLocations={state.event.candidate_custom_locations}
                  onRate={({ gmapsId, customId }) => navigation.navigate('RateLocation', { gmapsId, customId })}
                  onAdd={() => navigation.navigate('AddLocation', { eventId: state.event.id, groupId: state.group.id })}
                  onVote={() => navigation.navigate('VoteLocation', { eventId: state.event.id, groupId: state.group.id })}
                />
              </Card>
              <Card style={styles.section} elevation={0}>
                <Card.Title title="Candidate Times" />
                <Times
                  loading={loading}
                  times={state.event.candidate_times.map((t) => new Date(t))}
                  onVote={() => {
                    navigation.navigate('VoteTime', {
                      eventId: state.event.id
                    });
                  }}
                  onAddTime={(time) => {
                    setLoading(true);
                    return updateEvent({ state }, { ...state.event, candidate_times: [time] })
                      .then((event) => dispatch({ type: 'SET_EVENT', payload: event }))
                      .finally(() => setLoading(false));
                  }}
                  deadline={new Date(state.event.deadline)}
                />
              </Card>
              <Debts
                onAdd={() => {
                  navigation.navigate('AddDebt', {
                    eventId: state.event.id
                  });
                }}
                images={state.event.images || []}
              />
              {state.event.creator_id === state.user.id
                ? (
                  <ModalButton
                    style={{ backgroundColor: 'red' }}
                    modalText="Are you sure you want to delete the event?"
                    icon="delete-forever"
                    buttonText="DELETE"
                    confirmAction={() => {
                      deleteEvent({ state }, state.event.id).then(() => {
                        setDeleting(true);
                        dispatch({ type: 'REMOVE_EVENT', eventId: state.event.id });
                        navigation.goBack();
                      });
                    }}
                    confirmText="Delete"
                  />
                ) : null}
            </>
          )
          : (<ActivityIndicator />)}
      </ScrollView>
    </Surface>
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

export default EventView;

import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet } from 'react-native';
import { ScrollView } from 'react-native-gesture-handler';
import {
  ActivityIndicator, Card, Surface, Title
} from 'react-native-paper';
import {
  getEventInfo, updateEvent
} from '../../Requests';
import { store } from '../../Store';
import Creator from './creator';
import Deadline from './deadline';
import Debts from './debts';
import Locations from './locations';
import Times from './times';

const EventView = ({ navigation, route }) => {
  const { state } = useContext(store);
  const [eventDetails, setEventDetails] = useState(null);
  const [loading, setLoading] = useState(false);

  React.useLayoutEffect(() => {
    navigation.setOptions({ tabBarVisible: false });
  }, [navigation, route]);

  useEffect(() => {
    getEventInfo({ state }, state.event.id).then((event) => {
      setEventDetails({ ...event });
    });
  }, [state]);

  return (
    <Surface style={styles.container}>
      <ScrollView style={styles.container}>

        { eventDetails
          ? (
            <>
              <Title style={styles.eventTitle}>{state.event.name}</Title>
              <Deadline time={eventDetails.deadline} />
              <Creator userId={eventDetails.creator_id} />
              <Card style={styles.section} elevation={0}>
                <Card.Title title="Candidate Locations" />
                <Locations
                  googleLocations={eventDetails.candidate_google_maps_locations}
                  customLocations={eventDetails.candidate_custom_locations}
                  onAdd={() => navigation.navigate('AddLocation', { eventId: state.event.id, groupId: state.group.id })}
                  onVote={() => navigation.navigate('VoteLocation', { eventId: state.event.id, groupId: state.group.id })}
                />
              </Card>
              <Card style={styles.section} elevation={0}>
                <Card.Title title="Candidate Times" />
                <Times
                  loading={loading}
                  times={eventDetails.candidate_times.map((t) => new Date(t))}
                  onVote={() => {
                    navigation.navigate('VoteTime', {
                      eventId: state.event.id
                    });
                  }}
                  onAddTime={(time) => {
                    setLoading(true);
                    return updateEvent({ state }, { ...eventDetails, candidate_times: [time] }).then((r) => setEventDetails(r)).finally(() => setLoading(false));
                  }}
                  deadline={new Date(eventDetails.deadline)}
                />
              </Card>
              <Debts
                onAdd={() => {
                  navigation.navigate('AddDebt', {
                    eventId: state.event.id
                  });
                }}
                images={eventDetails.images || []}
              />
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

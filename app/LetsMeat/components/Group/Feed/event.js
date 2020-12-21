import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import {
  ActivityIndicator, Button, Card, Subheading, Surface, Title
} from 'react-native-paper';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import { getEventInfo, getUsersInfo } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

const Creator = ({ userId }) => {
  const { state } = useContext(store);
  const [userInfo, setUserInfo] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, userId).then((users) => {
      console.log(users);
      setUserInfo(users[0]);
    });
  }, []);

  return (
    userInfo
      ? <UserCard user={userInfo} />
      : <ActivityIndicator />
  );
};

const Deadline = ({ time }) => (
  <Card style={{ margin: 20 }}>

    <Subheading>
      {time}
    </Subheading>
  </Card>
);

const Locations = ({ locations }) => (
  <>
    {locations.map((l) => {
      <Card>{l}</Card>;
    })}
    <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
      <Button style={styles.addButton}>
        <Icon name="plus" size={25} />
      </Button>
      <Button style={styles.addButton}>
        <Icon name="vote" size={25} />
      </Button>
    </View>
  </>
);

const Times = ({ times }) => (
  <>
    {times.map((t) => {
      <Card>{t}</Card>;
    })}
    <Button style={styles.addButton}>
      <Text>Add time</Text>
    </Button>
  </>
);

const Debts = ({}) => (
  <Card style={styles.section}>
    <Card.Title title="Debts" />
  </Card>
);

const EventView = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [eventDetails, setEventDetails] = useState(null);
  console.log(eventDetails);

  useEffect(() => {
    console.log('using callback xD');
    getEventInfo({ state }, state.event.id).then((event) => {
      console.log(event);
      setEventDetails({ ...eventDetails, ...event });
    });
  }, []);

  return (
    <Surface style={styles.container}>
      { eventDetails
        ? (
          <>
            <Card style={styles.section}>
              <Title style={{ fontSize: 30, marginHorizontal: 20, marginTop: 20 }}>{state.event.name}</Title>
              <Deadline time={eventDetails.deadline} />
              <Creator userId={eventDetails.creator_id} />
            </Card>
            <Card style={styles.section}>
              <Card.Title title="Candidate Locations" />
              <Locations
                locations={[...eventDetails.candidate_google_maps_locations, ...eventDetails.candidate_custom_locations]}
                googleLocations={eventDetails.candidate_google_maps_locations}
                customLocations={eventDetails.candidate_custom_locations}
              />
            </Card>
            <Card style={styles.section}>
              <Card.Title title="Candidate Times" />
              <Times
                times={eventDetails.candidate_times}
              />
            </Card>
            <Debts />
          </>
        )
        : (<ActivityIndicator />)}
    </Surface>
  );
};

const styles = StyleSheet.create({
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
  }
});

export default EventView;

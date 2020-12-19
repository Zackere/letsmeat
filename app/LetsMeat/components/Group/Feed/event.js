import { useScrollToTop } from '@react-navigation/native';
import React, { useContext, useEffect, useState } from 'react';
import { StyleSheet, Text } from 'react-native';
import { Surface, Title, ActivityIndicator } from 'react-native-paper';
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

const Deadline = ({ time }) =>
  // placeholder
  (
    <Text>
      {time}
    </Text>
  );

const Locations = () => {

}

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
            <Title>{state.event.name}</Title>
            <Creator userId={eventDetails.creator_id} />
            <Deadline time={eventDetails.deadline} />
            <Title>Candidate Locations</Title>
            <Locations googleLocations={eventDetails.candidate_google_maps_locations}
              customLocations={eventDetails.candidate_custom_locations}></Locations>
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
  card: {
    margin: 25
  }
});

export default EventView;

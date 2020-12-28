import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import {
  Image, StyleSheet, View, Text
} from 'react-native';
import { ScrollView } from 'react-native-gesture-handler';
import ImagePicker from 'react-native-image-crop-picker';
import {
  ActivityIndicator, Button, Card, Surface, Title
} from 'react-native-paper';
import { CustomLocationCard, GMapsCard } from '../../Location';
import {
  getEventInfo, getImagesInfo, getUsersInfo, updateEvent, uploadImage, getLocationsInfo
} from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';
import Times, { TimeCard } from './times';

const Creator = ({ userId }) => {
  const { state } = useContext(store);
  const [userInfo, setUserInfo] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, userId).then((users) => {
      setUserInfo(users[0]);
      console.log(users[0]);
    });
  }, [state, userId]);

  return (
    <Card elevation={0} style={styles.section}>
      <Card.Title title="Creator" />
      {userInfo
        ? <UserCard user={userInfo} />
        : <ActivityIndicator />}
    </Card>
  );
};

const Deadline = ({ time }) => (
  <Card elevation={0} style={styles.section}>
    <Card.Title title="Deadline" />
    <TimeCard time={new Date(time)} />
  </Card>
);

const Locations = ({
  customLocations, googleLocations, onAdd, onVote
}) => {
  const { state } = useContext(store);
  const [loading, setLoading] = useState(true);
  const [locationsInfo, setLocationsInfo] = useState(null);

  useEffect(() => {
    setLoading(true);
    getLocationsInfo({ state }, customLocations, googleLocations)
      .then((info) => { setLocationsInfo(info); setLoading(false); });
  }, [customLocations, googleLocations, state]);

  return (
    loading ? <ActivityIndicator />
      : (
        <>
          <View>
            {locationsInfo.custom_location_infomation.map((l) => <CustomLocationCard location={l} key={l.id} />)}
            {locationsInfo.google_maps_location_information.map((l) => <GMapsCard location={l} key={l.details.place_id} />)}
          </View>
          <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
            <Button style={styles.addButton} onPress={onAdd}>
              <Icon name="plus" size={25} />
            </Button>
            <Button style={styles.addButton} onPress={onVote}>
              <Icon name="vote" size={25} />
            </Button>
          </View>
        </>
      )
  );
};

const DebtImage = ({ id }) => {
  const { state } = useContext(store);
  const [imageData, setImageData] = useState(null);
  const [imageSize, setImageSize] = useState({ width: 0, height: 0 });

  useEffect(() => {
    getImagesInfo({ state }, id).then((info) => {
      Image.getSize(info[0].image_url, (width, height) => {
        setImageSize({ width, height });
        setImageData(info[0]);
      }).catch(console.log);
    });
  }, [id]);

  return (
    imageData ? <Image style={{ width: '100%', height: undefined, aspectRatio: imageSize.width / imageSize.height || 0 }} source={{ uri: imageData.image_url }} /> : <ActivityIndicator />
  );
};

const Images = ({ images }) => (images ? images.map((i) => <DebtImage key={i} id={i} />) : <></>);

const Debts = ({ images }) => {
  const { state } = useContext(store);
  return (
    <Card style={styles.section}>
      <Card.Title title="Debts" />
      <Card.Content>
        <Images images={images} />
        <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
          <Button
            style={styles.addButton}
            onPress={
          () => {
            ImagePicker.openPicker({
              cropping: true
            }).then((i) => uploadImage({ state }, state.event.id, i));
          }
        }
          >
            <Icon name="image-plus" size={25} />
          </Button>
        </View>
      </Card.Content>
    </Card>
  );
};

const EventView = ({ navigation, route }) => {
  const { state, dispatch } = useContext(store);
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
                    // console.log({ ...eventDetails, candidate_times: [time] });
                    setLoading(true);
                    return updateEvent({ state }, { ...eventDetails, candidate_times: [time] }).then((r) => setEventDetails(r)).finally(() => setLoading(false));
                  }}
                  deadline={new Date(eventDetails.deadline)}
                />
              </Card>
              <Debts images={eventDetails.images || []} />
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

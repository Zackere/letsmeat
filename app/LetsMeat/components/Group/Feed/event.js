import React, { useContext, useEffect, useState } from 'react';
import {
  StyleSheet, Text, View, Image, ImageEditor
} from 'react-native';
import {
  ActivityIndicator, Button, Card, Subheading, Surface, Title
} from 'react-native-paper';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import ImagePicker from 'react-native-image-crop-picker';
import { ScrollView } from 'react-native-gesture-handler';
import {
  getEventInfo, getUsersInfo, uploadImage, getImagesInfo
} from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

const Creator = ({ userId }) => {
  const { state } = useContext(store);
  const [userInfo, setUserInfo] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, userId).then((users) => {
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

const EventView = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [eventDetails, setEventDetails] = useState(null);

  useEffect(() => {
    getEventInfo({ state }, state.event.id).then((event) => {
      setEventDetails({ ...eventDetails, ...event });
    });
  }, []);

  return (
    <Surface style={styles.container}>
      <ScrollView style={styles.container}>

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
              <Debts images={eventDetails.images || []} />
            </>
          )
          : (<ActivityIndicator />)}
      </ScrollView>
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

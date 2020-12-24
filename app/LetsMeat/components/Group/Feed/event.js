import React, { useContext, useEffect, useState } from 'react';
import {
  StyleSheet, Text, View, Image, ImageEditor
} from 'react-native';
import {
  ActivityIndicator, Button, Card, Subheading, Surface, Title, Headline, Caption
} from 'react-native-paper';
import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import ImagePicker from 'react-native-image-crop-picker';
import { ScrollView } from 'react-native-gesture-handler';
import DateTimePicker from '@react-native-community/datetimepicker';
import {
  getEventInfo, getUsersInfo, uploadImage, getImagesInfo, updateEvent
} from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

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

const formatTime = (time) => (`${time.getHours().toString().padStart(2, '0')}:${time.getMinutes().toString().padStart(2, '0')}`);

const formatDate = (time) => {
  const year = time.getFullYear(); // 2019
  const date = time.getDate(); //
  const months = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];
  const days = [
    'Sun',
    'Mon',
    'Tue',
    'Wed',
    'Thu',
    'Fri',
    'Sat'
  ];
  const monthName = months[time.getMonth()];
  const dayName = days[time.getDay()]; // Thu
  return `${dayName}, ${date} ${monthName} ${year}`;
};

const getTimeLeft = (timePoint, includeSeconds = false) => {
  const now = new Date();
  const msDiff = timePoint - now;
  const ms = Math.abs(msDiff);
  const days = Math.floor(ms / (1000 * 60 * 60 * 24));
  const hours = Math.floor(ms / (1000 * 60 * 60)) - 24 * days;
  const minutes = Math.floor(ms / (1000 * 60)) - 24 * 60 * days - 60 * hours;
  const seconds = Math.floor(ms / 1000) - 24 * 60 * 60 * days - 60 * 60 * hours - 60 * minutes;
  return `${days} days, ${hours} hours ${minutes} minutes ${includeSeconds ? `${seconds} seconds` : ''}${msDiff <= 0 ? 'ago' : 'left'}`;
};

const DateAndHourPicker = ({
  setValue, visible, setVisible, minimumDate, onDismiss
}) => {
  const [result, setResult] = useState(minimumDate);
  const [mode, setMode] = useState('date');

  const onChange = (event) => {
    setVisible(false);
    const { timestamp } = { ...event.nativeEvent };
    if (event.type === 'dismissed') {
      setMode('date');
      onDismiss();
      return;
    }
    if (mode === 'date') {
      setMode('time');
      setResult(new Date(timestamp));
      setVisible(true);
    }
    if (mode === 'time') {
      setMode('date');
      setValue(new Date(timestamp));
    }
  };

  return (visible
    ? (
      <DateTimePicker
        onChange={onChange}
        value={result}
        minimumDate={minimumDate}
        mode={mode}
        display="spinner"
        minuteInterval={5}
      />
    ) : null
  );
};

const TimeCard = ({ time, highlight = false }) => {
  const placeholder = 1;

  return (
    <Card style={styles.timeCard} elevation={highlight ? 5 : 1}>
      <View style={{ textAlign: 'center' }}>
        <Headline style={{ fontSize: 30, textAlign: 'center' }}>
          {formatTime(time)}
        </Headline>
        <Headline style={{ fontSize: 17, textAlign: 'center', lineHeight: 17 }}>
          {formatDate(time)}
        </Headline>
        <Caption style={{ textAlign: 'center', fontStyle: 'italic', lineHeight: 13 }}>
          {getTimeLeft(time)}
        </Caption>
      </View>
    </Card>
  );
};

const Times = ({
  times, deadline, onAddTime, loading
}) => {
  const [isAdding, setAdding] = useState(false);
  const [newDate, setNewDate] = useState(null);
  const [datePickerVisible, setDatePickerVisible] = useState(false);
  const onDismiss = () => {
    setNewDate(null);
    setAdding(false);
  };

  const THREE_MIN = 3 * 60 * 1000;

  const onSetDate = (date) => {
    times.forEach((t) => {
      if (Math.abs(t - date) <= THREE_MIN) {
        console.log('That date has already been added');
        onDismiss();
      }
    });
    setNewDate(date);
  };

  return (
    <>
      <View>
        {times.map((t) => <TimeCard key={t.getTime()} time={t} />)}
        {newDate ? <TimeCard key={newDate.getTime()} time={newDate} highlight /> : null}
      </View>
      <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
        {isAdding ? (
          <>
            <Button
              style={styles.addButton}
              onPress={() => {
                setAdding(false);
                onAddTime(newDate).finally(() => setNewDate(null));
              }}
            >
              <Icon name="check" size={25} />
            </Button>
            <Button
              style={styles.addButton}
              onPress={onDismiss}
            >
              <Icon name="close" size={25} />
            </Button>
          </>
        ) : (
          <>
            <Button
              style={styles.addButton}
              disabled={loading}
              onPress={() => {
                if (isAdding) return;
                setAdding(true);
                setDatePickerVisible(true);
              }}
            >
              <Icon name="plus" size={25} />
            </Button>
            <Button
              style={styles.addButton}
              disabled={loading}
              onPress={() => Promise.reject('Not implemented')}
            >
              <Icon name="vote" size={25} />
            </Button>
          </>
        )}
      </View>
      <DateAndHourPicker
        visible={datePickerVisible}
        setVisible={setDatePickerVisible}
        setValue={onSetDate}
        onDismiss={onDismiss}
        minimumDate={deadline}
      />
    </>
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

const EventView = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [eventDetails, setEventDetails] = useState(null);
  const [loading, setLoading] = useState(false);

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
                  loading={loading}
                  times={eventDetails.candidate_times.map((t) => new Date(t))}
                  onAddTime={(time) => {
                    console.log({ ...eventDetails, candidate_times: [time] });
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

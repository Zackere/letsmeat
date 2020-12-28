import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import DateTimePicker from '@react-native-community/datetimepicker';
import React, { useState } from 'react';
import {
  StyleSheet, View
} from 'react-native';
import {
  Button, Caption, Card, Headline
} from 'react-native-paper';
import { formatDate, formatTime, getTimeLeft } from '../../../helpers/time';

export const TimeCard = ({ time, highlight = false, onLongPress }) => (
  <Card style={styles.timeCard} elevation={highlight ? 5 : 1} onLongPress={onLongPress}>
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

export const DateAndHourPicker = ({
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

const Times = ({
  times, deadline, onAddTime, loading, onVote
}) => {
  const [isAdding, setAdding] = useState(false);
  const [newDate, setNewDate] = useState(null);
  const [datePickerVisible, setDatePickerVisible] = useState(false);
  const [isVoting, setIsVoting] = useState(false);

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
      {isVoting
        ? (
          <View>
            {times.map((t) => <TimeCard key={t.getTime()} time={t} />)}
            {newDate ? <TimeCard key={newDate.getTime()} time={newDate} highlight /> : null}
          </View>
        ) : (
          <View>
            {times.map((t) => <TimeCard key={t.getTime()} time={t} />)}
            {newDate ? <TimeCard key={newDate.getTime()} time={newDate} highlight /> : null}
          </View>
        )}
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
              color="red"
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
              disabled={loading || times.length === 0}
              onPress={onVote}
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

export default Times;

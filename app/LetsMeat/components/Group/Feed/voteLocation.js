import React, {
  useState, useEffect, useContext, useLayoutEffect,
  useRef
} from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import { Surface, ActivityIndicator } from 'react-native-paper';
import DraggableFlatList from 'react-native-draggable-flatlist';
import { TimeCard } from './times';
import { store } from '../../Store';
import { castVote, getVoteTimes } from '../../Requests';

const VoteLocation = ({ navigation, route }) => {
  const { state } = useContext(store);

  const { eventId } = route.params;
  const [loading, setLoading] = useState(true);
  const [voting, setVoting] = useState(false);
  const [times, setTimes] = useState([]);

  const setHeaderAction = () => {
    if (!loading && times && times.length > 0 && !voting) {
      navigation.setOptions({
        rightIcon: 'vote',
        rightAction: () => {
          setVoting(true);
          castVote({ state }, eventId, times).then(() => {
            setVoting(false);
          });
        }
      });
    } else if (voting) {
      navigation.setOptions({
        rightIcon: 'timer-sand',
        rightAction: () => {}
      });
    }
  };

  useLayoutEffect(setHeaderAction, [times, loading, state, navigation, voting, eventId]);

  useEffect(() => {
    getVoteTimes({ state }, eventId).then((data) => {
      setTimes(data);
      setLoading(false);
    });
  }, [state, eventId]);

  return (
    <Surface style={styles.container}>
      { loading ? <ActivityIndicator />
        : (
          <DraggableFlatList
            data={times}
            renderItem={({
              item, drag, isActive
            }) => <TimeCard time={new Date(item)} highlight={isActive} onLongPress={drag} />}
            keyExtractor={(item) => `${item}`}
            onDragEnd={({ data }) => setTimes(data)}
          />
        )}
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

export default VoteLocation;

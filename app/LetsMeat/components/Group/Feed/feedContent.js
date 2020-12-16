import React, { useContext } from 'react';
import {
  Text, View, StyleSheet, ScrollView, RefreshControl
} from 'react-native';
import { Card, Surface } from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { TouchableHighlight } from 'react-native-gesture-handler';
import { Header } from '../Header';
import { store } from '../../Store';
import { getGroupInfo } from '../../Requests';

const Event = ({ event, onPress }) => (
  <Card
    key={event.id}
    style={styles.card}
    onPress={onPress}
  >
    <Card.Title title={event.name} />
    <Card.Content>
      <Text>
        {event.deadline}
      </Text>
    </Card.Content>
  </Card>
);

const FeedContent = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [refreshing, setRefreshing] = React.useState(false);

  const onRefresh = () => {
    setRefreshing(true);
    getGroupInfo({ state }, state.group.id).then((group) => {
      setRefreshing(false);
      dispatch({ type: 'SET_GROUP', payload: group });
    });
  };

  return (
    <Surface style={styles.container}>
      <ScrollView
        // styles={styles.container}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
      }
      >
        {state.group.events && state.group.events.map((e) => (
          <Event
            key={e.id}
            event={e}
            onPress={() => {
              dispatch({ type: 'SET_EVENT', payload: e });
              navigation.navigate('Event');
            }}
          />
        ))}
      </ScrollView>
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

export default FeedContent;

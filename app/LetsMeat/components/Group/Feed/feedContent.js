import React, { useContext } from 'react';
import {
  RefreshControl, ScrollView, StyleSheet, Text, View
} from 'react-native';
import {
  Card, FAB, Surface, Title
} from 'react-native-paper';
import { useFocusEffect } from '@react-navigation/native';
import { getGroupInfo, getGroupDebts } from '../../Requests';
import { store } from '../../Store';
import { TimeCard } from './times';
import BackgroundContainer, { ScrollPlaceholder } from '../../Background';

const Event = ({ event, onPress }) => (
  <Card elevation={1} style={{ margin: 10, backgroundColor: 'rgba(230, 230, 230, 0.9)' }} onPress={onPress}>
    <Card.Content>
      <View style={{ margin: 20, marginTop: 10 }}>
        <Title style={{ fontSize: 30 }}>{event.name}</Title>
      </View>
      <TimeCard time={new Date(event.deadline)} />
    </Card.Content>
  </Card>
);

const FeedContent = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [refreshing, setRefreshing] = React.useState(false);

  const onRefresh = () => {
    setRefreshing(true);
    Promise.all(
      [getGroupInfo({ state }, state.group.id),
        getGroupDebts({ state }, state.group.id)]
    )
      .then(([groupInfo, debtInfo]) => {
        setRefreshing(false);
        console.log(debtInfo);
        dispatch({ type: 'SET_GROUP', payload: { ...groupInfo, ...debtInfo } });
      });
    return () => {};
  };

  // useFocusEffect(React.useCallback(() => { onRefresh(); }, [state.group.id]));

  return (
    <BackgroundContainer>
      <ScrollView
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
      >
        <>
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
          <ScrollPlaceholder height={200} />
        </>
      </ScrollView>
      <FAB
        style={styles.fab}
        icon="plus"
        label="New Event"
        onPress={() => { navigation.navigate('NewEvent'); }}
      />
    </BackgroundContainer>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  },
  card: {
    margin: 25
  },
  fab: {
    margin: 30,
    position: 'absolute',
    bottom: 100,
    right: 0
  }
});

export default FeedContent;

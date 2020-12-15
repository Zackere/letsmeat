import React, { useEffect, useState, useContext } from 'react';
import {
  Text, View, StyleSheet, FlatList
} from 'react-native';
import {
  FAB, Card, Paragraph, Surface, IconButton
} from 'react-native-paper';
import { ScrollView, TouchableHighlight } from 'react-native-gesture-handler';
import * as Progress from 'react-native-progress';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { getGroups, getGroupInfo } from '../../Requests';
import { store } from '../../Store';

const GroupButton = (group, navigation) => (
  <TouchableHighlight
    onPress={() => navigation.navigate('Group')}
  >
    <Text>
      Group xD
    </Text>
  </TouchableHighlight>
);

const CreateGroupButton = ({ onPress }) => (
  <TouchableHighlight
    onPress={onPress}
  >
    <Text>
      Touch to create a new Group
    </Text>
  </TouchableHighlight>
);

const RenderGroup = ({
  item, separators, navigation, dispatch, state
}) => {
  const [additionalGroupInfo, setAdditionalGroupInfo] = useState(null);
  return (
    <Card
      style={styles.emptyCard}
      elevation={3}
      onPress={() => {
        console.log('wating')
        getGroupInfo({ state }, item.id)
          .then((group) => {
            dispatch({ type: 'SET_GROUP', payload: group });
            navigation.navigate('Home');
          }).catch(console.log);
      }}
    >
      <Card.Title title={item.name} />
      <Card.Content>
        <Paragraph>
          Some kind of description
        </Paragraph>
      </Card.Content>
    </Card>
  );
};

export const Groups = ({ navigation, route }) => {
  const load = () => {
    setLoadingGroups(true);
    getGroups({ state, dispatch }).catch(console.log).then(onGetGroups);
  };

  navigation.addListener('focus', load);

  const [groups, setGroups] = useState([]);
  const { state, dispatch } = useContext(store);
  const [loadingGroups, setLoadingGroups] = useState(true);

  const onGetGroups = (groups) => {
    setGroups(groups || []);
    setLoadingGroups(false);
  };

  useEffect(() => {
    getGroups({ state, dispatch }).catch(console.log).then(onGetGroups);
  }, []);

  return (
    loadingGroups
      ? (
        <View style={styles.progressBar}>
          <Progress.Bar size={30} indeterminate />
        </View>
      )
      : (
        <>
          <Surface style={styles.groupsContainer}>
            <FlatList
              data={groups}
              renderItem={({ item, separators }) => (
                <RenderGroup
                  item={item}
                  separators={separators}
                  navigation={navigation}
                  dispatch={dispatch}
                  state={state}
                />
              )}
              ListEmptyComponent={() => (
                <Card
                  style={styles.emptyCard}
                  elevation={10}
                >
                  <Card.Title title="Nothing to show" />
                  <Card.Cover source={{ uri: 'https://images.unsplash.com/photo-1577460551100-907ba84418ce?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1274&q=80' }} />

                  <Card.Content>
                    <Paragraph>
                      Consider adding a group
                    </Paragraph>
                  </Card.Content>
                </Card>
              )}
            />
          </Surface>
          <FAB
            style={styles.fab}
            icon="plus"
            label="Create new group"
            onPress={() => {
              navigation.navigate('CreateGroup');
              setLoadingGroups(true);
            }}
          />
        </>
      )
  );
};

const styles = StyleSheet.create({
  progressBar: {
    width: '100%',
    height: '100%',
    justifyContent: 'center',
    alignItems: 'center'
  },
  groupsContainer: {
    width: '100%',
    height: '100%'
  },
  fab: {
    position: 'absolute',
    margin: 30,
    right: 0,
    bottom: 0,
  },
  emptyCard: {
    margin: 25
  }
});

export default Groups;

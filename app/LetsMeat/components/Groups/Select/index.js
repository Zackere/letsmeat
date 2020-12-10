import React, { useEffect, useState, useContext } from 'react';
import {
  Text, View, StyleSheet
} from 'react-native';
import { FAB } from 'react-native-paper';
import { ScrollView, TouchableHighlight } from 'react-native-gesture-handler';
import * as Progress from 'react-native-progress';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { getGroups } from '../../Requests';
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

export const Groups = ({ navigation }) => {
  const [groups, setGroups] = useState([]);
  const { state, dispatch } = useContext(store);
  const [loadingGroups, setLoadingGroups] = useState(true);

  const onGetGroups = (groups) => {
    setGroups(groups || []);
    setLoadingGroups(false);
  };

  useEffect(() => {
    // console.log(state)
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
          <ScrollView style={styles.groupsContainer}>
            {
      groups.map((g) => (
        <GroupButton group={g} navigation={navigation} />
      ))
    }
          </ScrollView>
          <FAB
            style={styles.fab}
            icon="plus"
            label="Create new Group"
            onPress={() => console.log('Pressed')}
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
    fontSize: 50,
    margin: 50
  },
  fab: {
    position: 'absolute',
    margin: 30,
    right: 0,
    bottom: 0,
  }
});

export default Groups;

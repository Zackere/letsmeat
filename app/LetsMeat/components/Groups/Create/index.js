import React, { useEffect, useState, useContext } from 'react';
import {
  Text, View, StyleSheet, ToastAndroid
} from 'react-native';
import { ScrollView, TouchableHighlight } from 'react-native-gesture-handler';
import * as Progress from 'react-native-progress';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { getAPIToken, getGroups } from '../../Requests';
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

const CreateGroupButton = ({ onPress }) => {
  // console.log(onPress);
  return (
    <TouchableHighlight
      onPress={onPress}
    >
      <Text>
        Touch to create a new Group
      </Text>
    </TouchableHighlight>
  );
};

export const Create = ({ navigation }) => {


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
  }
});

export default Groups;

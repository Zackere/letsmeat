import { MaterialCommunityIcons } from '@expo/vector-icons';
import { useFocusEffect } from '@react-navigation/native';
import React, { useContext, useEffect, useState } from 'react';
import {
  FlatList, StyleSheet, Text, View
} from 'react-native';
import { TouchableHighlight } from 'react-native-gesture-handler';
import {
  Card, FAB, Paragraph, Surface, Badge, ActivityIndicator
} from 'react-native-paper';
import BackgroundContainer from '../../Background';
import { getGroupInfo, getGroups } from '../../Requests';
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
  groupId, navigation,
}) => {
  const { state, dispatch } = useContext(store);
  const group = state.groups && state.groups.find((g) => g.id === groupId);

  useEffect(() => {
    if (group && !group.custom_locations) getGroupInfo({ state }, groupId).then((info) => dispatch({ type: 'UPDATE_GROUP_INFO', groupId, groupInfo: info }));
  }, []);

  return (
    <Card
      style={styles.emptyCard}
      onPress={() => {
        getGroupInfo({ state }, groupId)
          .then((group) => {
            dispatch({ type: 'SET_GROUP', payload: group });
            navigation.navigate('Home', { screen: 'Feed' });
          }).catch(console.log);
      }}
    >
      <Card.Title title={group && group.name} />
      <Card.Content>
        <View style={{
          width: '100%', alignItems: 'flex-start', margin: 0, flexDirection: 'row'
        }}
        >
          <MaterialCommunityIcons size={30} name="emoticon-outline" />
          <Badge
            size={20}
            style={{ marginLeft: -10, marginTop: 10, fontSize: 15 }}
          >
            {group.users && group.users.length}
          </Badge>
          <MaterialCommunityIcons size={30} name="map-marker-outline" />
          <Badge
            size={20}
            style={{ marginLeft: -10, marginTop: 10, fontSize: 15 }}
          >
            {group.custom_locations && group.custom_locations.length}
          </Badge>
        </View>
      </Card.Content>
    </Card>
  );
};

export const Groups = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const groupsLoaded = state.groups;
  useFocusEffect(() => {
    if (groupsLoaded) setLoadingGroups(false);
  });
  const [loadingGroups, setLoadingGroups] = useState(true);

  useEffect(() => {
    if (!groupsLoaded) {
      getGroups({ state, dispatch }).then((groups) => {
        console.log(groups);
        dispatch({ type: 'SET_GROUPS', payload: groups });
        setLoadingGroups(false);
      });
    } else setLoadingGroups(false);
  }, [state, groupsLoaded, dispatch]);

  return (
    loadingGroups
      ? (
        <View style={styles.progressBar}>
          <ActivityIndicator />
        </View>
      )
      : (
        <>
          <BackgroundContainer backgroundVariant="office">
            <FlatList
              data={state.groups}
              renderItem={({ item, separators }) => (
                <RenderGroup
                  groupId={item.id}
                  separators={separators}
                  navigation={navigation}
                />
              )}
              ListEmptyComponent={() => (
                <Card
                  style={styles.emptyCard}
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
          </BackgroundContainer>
          <FAB
            style={styles.fab}
            icon="plus"
            label="Create new group"
            onPress={() => {
              setLoadingGroups(true);
              navigation.navigate('CreateGroup');
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
